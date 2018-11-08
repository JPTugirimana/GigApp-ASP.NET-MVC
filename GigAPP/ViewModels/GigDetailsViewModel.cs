using GigAPP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GigAPP.ViewModels
{
    public class GigDetailsViewModel
    {
        public Gig Gig { get; set; }
        public bool IsAttending { get; set; }
        public bool IsFollowing { get; set; }
    }
}