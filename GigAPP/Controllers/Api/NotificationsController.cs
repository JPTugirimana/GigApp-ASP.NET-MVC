using GigAPP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Web.Http;
using GigAPP.Dtos;
//using AutoMapper;

namespace GigAPP.Controllers.Api
{
    [Authorize]
    public class NotificationsController : ApiController
    {
        private ApplicationDbContext _context;
        

        public NotificationsController()
        {
            _context = new ApplicationDbContext();
            
        }
        
        [HttpPost]
        public IHttpActionResult MarkAsRead()
        {
            var userId = User.Identity.GetUserId();

            var notifications = _context.UserNotifications
                                         .Where(un => un.UserId == userId && !un.IsRead)
                                         .ToList();

            notifications.ForEach(n => n.Read());

            _context.SaveChanges();

            return Ok();
        }

        public IEnumerable<NotificationDto> GetNewNotifications()
        {
            var userId = User.Identity.GetUserId();

            var notifications = _context.UserNotifications
                                        .Where(un => un.UserId == userId && !un.IsRead)
                                        .Select(un => un.Notification)
                                        .Include(n => n.Gig.Artist)
                                        .ToList();

            // using Linq to map the notifications to notificationDto
            return notifications.Select(n => new NotificationDto()
            {
                DateTime = n.DateTime,
                Gig = new GigDto
                {
                    Artist = new UserDto
                    {
                        Id = n.Gig.Artist.Id,
                        name = n.Gig.Artist.Name
                    },
                    DateTime = n.Gig.DateTime,
                    Id = n.Gig.Id,
                    IsCanceled = n.Gig.IsCanceled,
                    Venue = n.Gig.Venue
                },
                OriginalDateTime = n.OriginalDateTime,
                OriginalVenue = n.OriginalVenue,
                Type = n.Type
            });


            /* Needs Fix ... Microsoft changed implementation ways
           //**** Use the package console to install automapper tool to get rid of redudancy 
            // in the mapping...install-package AutoMapper

            var config = new MapperConfiguration(cfg =>
             {
                cfg.CreateMap<ApplicationUser, UserDto>();
                cfg.CreateMap<Gig, GigDto>();
                cfg.CreateMap<Notification, NotificationDto>();
             });

            return notifications.Select(Mapper.Map<Notification, NotificationDto>); 
             
             */

        }
    }
}
