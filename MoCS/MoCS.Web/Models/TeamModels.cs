using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MoCS.Data.Entity;

namespace MoCS.Web.Models
{
    public class TeamModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public int Score { get; set; }

        [Required, Display(Name = "Admin user")]
        public string AdminUser { get; set; }

        public string AdminUserName { get; set; }
        public IEnumerable<ApplicationUser> Users;

        public Team ToTeam()
        {
            return new Team
            {
                Name = Name,
                Description = Description,
                AdminUser = AdminUser,
                CreateDateTime = DateTime.Now,
                Score = 0
            };
        }

        public TeamModel() { }

        public TeamModel(Team team, string adminUserName = "")
        {
            Id = team.Id;
            Name = team.Name;
            Description = team.Description;
            AdminUser = team.AdminUser;
            AdminUserName = adminUserName;
        }

    }
}