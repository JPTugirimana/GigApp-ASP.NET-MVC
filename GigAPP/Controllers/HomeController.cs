using GigAPP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using GigAPP.ViewModels;
using GigAPP.MyCodeRepository;

namespace GigAPP.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext _context;
        private UserFollowings _userFollowings;

        public HomeController()
        {
            _context = new ApplicationDbContext();
            _userFollowings = new UserFollowings(_context);
        }

        public ActionResult Index(string querry = null)
        {
            //get from gig database all gig with future date and load artist genre
            var upcomingGigs = GetAllFutureUncanceledGigs(); 

            if(!String.IsNullOrWhiteSpace(querry)){
                upcomingGigs = upcomingGigs
                                .Where(g =>
                                            g.Artist.Name.Contains(querry) ||
                                            g.Genre.Name.Contains(querry) ||
                                            g.Venue.Contains(querry));
            }

            var userId = User.Identity.GetUserId();
            var followings = _userFollowings.GetArtistsUserIsFollowing(userId)
                                            .ToLookup(a => a.FolloweeId);

            var viewModel = new GigsViewModel
            {
                UpcomingGigs = upcomingGigs,
                ShowActions = User.Identity.IsAuthenticated,
                Heading = "Upcoming Gigs:",
                SearchTerm = querry,
                Attendances = GetGigsUserIsAttending(userId).ToLookup(a => a.GigId),
                Followings = followings,
                CurrentUser = userId
            };
            
            return View("Gigs", viewModel);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        // Private Methods to keep the code cleaner
        //===========================================

        private List<Attendance> GetGigsUserIsAttending(string userId){
            return _context.Attendances
                           .Where(a => a.AttendeeId == userId && a.Gig.DateTime > DateTime.Now)
                           .ToList();
        }

        private IEnumerable<Gig> GetAllFutureUncanceledGigs()
        {
            return _context.Gigs
                 .Include(g => g.Artist)
                 .Include(g => g.Genre)
                 .Where(g => g.DateTime > DateTime.Now && !g.IsCanceled);
        }

        //******END of Private Methods********
    }
}