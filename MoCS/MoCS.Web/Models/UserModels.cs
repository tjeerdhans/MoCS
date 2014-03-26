using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MoCS.Data.Entity;

namespace MoCS.Web.Models
{
    public class UserModel
    {
        public IEnumerable<UserModelItem> UserModelItems { get; set; }

        public UserModel()
        {
            UserModelItems = new List<UserModelItem>();
        }
    }

    public class UserModelItem
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string TeamName { get; set; }

        public UserModelItem(ApplicationUser user, Team team)
        {
            Id = user.Id;
            UserName = user.UserName;
            TeamName = team.Name;
        }
    }
}