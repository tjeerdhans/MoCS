﻿using MoCS.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

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
        public string AdminUserId { get; set; }
        public string AdminUserName { get; set; }
        public IEnumerable<SelectListItem> AdminUsersSelectListItems { get; set; }

        [Display(Name = "Team members")]
        public List<string> TeamMemberIds { get; set; }
        public List<string> TeamMemberNames { get; set; }
        public IEnumerable<SelectListItem> TeamMemberUsersSelectListItems { get; set; }

        public TeamModel()
        {
            TeamMemberIds = new List<string>();
        }

        public void FillAdminUsersSelectListItems(IEnumerable<ApplicationUser> adminUsers)
        {
            AdminUsersSelectListItems = adminUsers.Select(applicationUser => new SelectListItem
            {
                Value = applicationUser.Id,
                Text = applicationUser.UserName,
                Selected = applicationUser.Id == AdminUserId
            }).ToList();
        }

        public void FillTeamMemberUsersSelectListItems(IEnumerable<ApplicationUser> teamMemberUsers)
        {
            TeamMemberUsersSelectListItems = teamMemberUsers.Select(applicationUser => new SelectListItem
            {
                Value = applicationUser.Id,
                Text = applicationUser.UserName,
                Selected = TeamMemberIds.Contains(applicationUser.Id)
            }).ToList();
        }

        public void UpdateTeam(ref Team team)
        {
            Name = Name.Trim();
            Description = Description.Trim();

            var dirty = team.Name != Name
                        || team.Description != Description
                        || team.AdminUser != AdminUserId;

            team.Name = Name;
            team.Description = Description;
            team.AdminUser = AdminUserId;

            if (dirty) team.LastModified = DateTime.UtcNow;
        }

        public TeamModel(Team team, string adminUserName = "")
        {
            Id = team.Id;
            Name = team.Name;
            Description = team.Description;
            AdminUserId = team.AdminUser;
            AdminUserName = adminUserName;
        }
    }
}