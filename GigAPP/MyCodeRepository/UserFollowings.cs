using GigAPP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GigAPP.MyCodeRepository
{
    public class UserFollowings
    {
        private readonly ApplicationDbContext _context;

        public UserFollowings(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Following> GetArtistsUserIsFollowing(string userId)
        {
            return _context.Followings
                           .Where(f => f.FollowerId == userId)
                            .ToList();
                                       
        }
    }
}