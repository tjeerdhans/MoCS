using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using MoCS.Data.Entity;

namespace MoCS.Web.Models
{
    public class TournamentModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public DateTime CreateDateTime { get; set; }

        [Display(Name = "Assignments in this tournament")]
        public List<string> AssignmentIds { get; set; }
        public List<string> AssignmentNames { get; set; }
        public IEnumerable<SelectListItem> AssignmentSelectListItems { get; set; }

        public TournamentModel()
        {
            AssignmentIds = new List<string>();
        }

        public void FillAssignmentSelectListItems(IEnumerable<Assignment> assignments)
        {
            AssignmentSelectListItems = assignments.Select(assignment => new SelectListItem
            {
                Value = assignment.Id.ToString(CultureInfo.InvariantCulture),
                Text = assignment.FriendlyName,
                Selected = AssignmentIds.Contains(assignment.Id.ToString(CultureInfo.InvariantCulture))
            }).ToList();
        }

        public Tournament ToTournament()
        {
            return new Tournament
            {
                Name = Name,
                CreateDateTime = DateTime.Now
            };
        }

        public TournamentModel(Tournament tournament)
        {
            Id = tournament.Id;
            Name = tournament.Name;
            CreateDateTime = tournament.CreateDateTime;
            AssignmentIds = tournament.TournamentAssignments.Select(ta => ta.Assignment.Id.ToString(CultureInfo.InvariantCulture)).ToList();
        }
    }
}