using GigAPP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using System.Data.Entity;

namespace GigAPP.Controllers.Api
{
    [Authorize]
    public class GigsController : ApiController
    {
        private ApplicationDbContext _context;
        
        public GigsController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpDelete] //because we want it to be called only using 'delete' verb
        public IHttpActionResult Cancel(int id)
        {
            var userId = User.Identity.GetUserId();
            var gig = _context.Gigs
                              .Include(g => g.Attendances.Select(a => a.Attendee)) // the include method used here required importing system.data.entity
                              .Single(g => g.Id == id  && g.ArtistId == userId);
            if (gig.IsCanceled)
                return NotFound();

            gig.Cancel();

            _context.SaveChanges();

            return Ok();
        }
    }
}
