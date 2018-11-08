using GigAPP.Models;
using GigAPP.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Data.Entity; // For include method
using GigAPP.MyCodeRepository; 

namespace GigAPP.Controllers
{
    public class GigsController : Controller
    {
        // Get a list of Genres from the database in order to display them in the dropdown
        private ApplicationDbContext _context;

        // For my created repository: purpose of reusing codes/private methods
        private UserFollowings _userFollowings;

        // create a construct to initialize
        public GigsController(){
            _context = new ApplicationDbContext();
            _userFollowings = new UserFollowings(_context);
        }

        public ActionResult Details(int id) {
            var gig = GetTheGig(id); 

            if (gig == null)
                return HttpNotFound();

            var viewModel = new GigDetailsViewModel{Gig = gig};

            if(User.Identity.IsAuthenticated) {
                var userId = User.Identity.GetUserId();

                viewModel.IsAttending = _context.Attendances
                                                .Any(a => a.AttendeeId == userId && a.GigId == gig.Id);

                viewModel.IsFollowing = _context.Followings
                                                .Any(f => f.FollowerId == gig.ArtistId && f.FolloweeId == userId);
            };

            return View(viewModel);
        }

        [Authorize]
        public ActionResult Mine()
        {
            var userId = User.Identity.GetUserId();
            var gigs = _context.Gigs
                               .Where(g => g.ArtistId == userId && g.DateTime > DateTime.Now && !g.IsCanceled)
                               .Include(g => g.Genre)
                               .ToList();

            return View(gigs);
        }

        // JP's ADDON FEATURE:
        // an action for the artists to see all the gigs they ever created (all gigs in the database) 
        // including the ones they cancelled.
        [Authorize]
        public ActionResult AllMine()
        {
            var userId = User.Identity.GetUserId();
            var gigs = _context.Gigs
                               .Where(g => g.ArtistId == userId)
                               .Include(g => g.Genre)
                               .ToList();

            return View(gigs);
        }

        [Authorize]
        public ActionResult Attending()
        {
            var userId = User.Identity.GetUserId();
            var gigs = GetGigsUserIsAttending(userId);

            var followings = _userFollowings.GetArtistsUserIsFollowing(userId)
                                            .ToLookup(a => a.FolloweeId);

            var viewModel = new GigsViewModel()
            {
                UpcomingGigs = gigs,
                ShowActions = User.Identity.IsAuthenticated,
                Heading = "Gigs I'm attending:",
                Attendances = GetUserFutureAttendances(userId).ToLookup(a => a.GigId),
                Followings = followings
            };

            return View("Gigs",viewModel);
        }

        //private List<ApplicationDbContext> 

        [Authorize]
        public ActionResult Following()
        {
            var userId = User.Identity.GetUserId();
            var artists = _context.Followings
                .Where(f => f.FollowerId == userId)
                .Include(f => f.Followee)
                .ToList();

            /*var viewModel = new GigsViewModel()
            {
                UpcomingGigs = gigs,
                ShowActions = User.Identity.IsAuthenticated,
                Heading = "Artits I'm following:"
            };*/ // NO need of using GigsViewModel because we just need to pass artists list...no other data/inputs

            return View("Artists", artists);
        }

    [HttpPost]
    public ActionResult Search(GigsViewModel ViewModel)
    {
        return RedirectToAction("Index", "Home", new {querry = ViewModel.SearchTerm });

    }

        [Authorize]  // add so it requires login & authorization to create a gig
        public ActionResult Create()
        {
            var ViewModel = new GigFormViewModel
            {
                // get a list of genres from the Genres database table
                Genres = _context.Genres.ToList(),    
                Heading = "Add Gig"
            };
            return View("GigForm", ViewModel);
        }

        [Authorize]  
        public ActionResult Edit(int id)
        {
            var userId = User.Identity.GetUserId();
            var gig = _context.Gigs.Single(g => g.Id == id && g.ArtistId == userId);

            var ViewModel = new GigFormViewModel
            {
                // get a list of genres from the Genres database table
                Heading = "Edit Gig",
                Id = gig.Id,
                Genres = _context.Genres.ToList(),
                Date = gig.DateTime.ToString("d MMM yyyy"),
                Time = gig.DateTime.ToString("HH:mm"),
                Genre = gig.GenreId,
                Venue = gig.Venue
            };

            return View("GigForm", ViewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GigFormViewModel ViewModel)
        {
            if (!ModelState.IsValid)
            {
                // get a list of genres from the Genres database table
                ViewModel.Genres = _context.Genres.ToList();
                ViewModel.Heading = "Add Gig";

                return View("GigForm", ViewModel);  // the returned view (GigFormView needs Genres to be populated again
            }
                
            
            var gig = new Gig 
            {
                // Temporary holder for artist Id from the database;
                ArtistId = User.Identity.GetUserId(), 
                DateTime = ViewModel.GetDateTime(),
                GenreId = ViewModel.Genre,
                Venue = ViewModel.Venue,
            };

            // Execute SQL statement and save it the database
            _context.Gigs.Add(gig);
            _context.SaveChanges();

            //Then redirect the user back to the home page
            return RedirectToAction ("Mine", "Gigs");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(GigFormViewModel ViewModel)
        {
            if (!ModelState.IsValid)
            {
                // get a list of genres from the Genres database table
                ViewModel.Genres = _context.Genres.ToList();

                return View("GigForm", ViewModel);  // the returned view (GigFormView needs Genres to be populated again
            }


            var userId = User.Identity.GetUserId();
            var gig = _context.Gigs
                                .Include(g => g.Attendances.Select(a => a.Attendee))
                                .Single(g => g.Id == ViewModel.Id && g.ArtistId == userId);

            gig.Modify(ViewModel.GetDateTime(), ViewModel.Venue, ViewModel.Genre );

            _context.SaveChanges();

            //Then redirect the user back to the home page
            return RedirectToAction("Mine", "Gigs");
        }

        //====PRIVATE Methods to simplify the code...to make it self-explanatory & clean
        //==============================================================================

        private Gig GetTheGig(int id){ 
            
            return _context.Gigs
                           .Include(g => g.Artist)
                           .Include(g => g.Genre)
                           .SingleOrDefault(g => g.Id == id);
        }

        private List<Gig> GetGigsUserIsAttending(string userId)
        {
            return _context.Attendances
                 .Where(a => a.AttendeeId == userId)
                 .Select(a => a.Gig)
                 .Include(g => g.Artist)
                 .Include(g => g.Genre)
                 .ToList();
        }

        private List<Attendance> GetUserFutureAttendances(string userId)
        {
            return _context.Attendances
                           .Where(a => a.AttendeeId == userId && a.Gig.DateTime > DateTime.Now)
                           .ToList();
                                         
        }


        //=======END of Private Mehtods
    }
}