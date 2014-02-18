using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using MoCS.Data.Entity;

namespace MoCS.Web.Models
{
    public class TeamModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Score { get; set; }
        public Guid AdminUser { get; set; }
        public IEnumerable<ApplicationUser> Users;

        public Team ToTeam()
        {
            return new Team
            {
                Name = Name,
                Description = Description,
                AdminUser = 0,
                CreateDateTime = DateTime.Now,
                Score = 0
            };
        }

        public TeamModel() { }

        public TeamModel(Team team)
        {
            Name = team.Name;
            Description = team.Description;
            AdminUser = Guid.NewGuid();
        }

    }
}