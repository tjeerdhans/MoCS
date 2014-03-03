using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using MoCS.Data.Entity;
using WebGrease.Css.Extensions;

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
        public List<SelectListItem> AssignmentSelectListItems { get; set; }

        public TournamentModel()
        {
            AssignmentIds = new List<string>();
            AssignmentSelectListItems = new List<SelectListItem>();
        }

        public TournamentModel(Tournament tournament) : this()
        {
            Id = tournament.Id;
            Name = tournament.Name;
            CreateDateTime = tournament.CreateDateTime;
            AssignmentIds = tournament.TournamentAssignments.OrderBy(ta => ta.AssignmentOrder).Select(ta => ta.Assignment.Id.ToString(CultureInfo.InvariantCulture)).ToList();
        }

        public void FillAssignmentSelectListItems(List<Assignment> assignments)
        {
            // add the selected items in the correct order first
            foreach (var assignmentId in AssignmentIds)
            {
                var id = int.Parse(assignmentId);
                AssignmentSelectListItems.Add(AssignmentToSelectListItem(assignments.Single(a => a.Id == id), true));
            }
            assignments
                .Where(a => !AssignmentIds.Contains(a.Id.ToString(CultureInfo.InvariantCulture)))
                .ForEach(a => AssignmentSelectListItems.Add(AssignmentToSelectListItem(a, false)));
            //AssignmentSelectListItems = assignments.Select(assignment => new SelectListItem
            //{
            //    Value = assignment.Id.ToString(CultureInfo.InvariantCulture),
            //    Text = assignment.FriendlyName,
            //    Selected = AssignmentIds.Contains(assignment.Id.ToString(CultureInfo.InvariantCulture))
            //}).ToList();
        }

        private SelectListItem AssignmentToSelectListItem(Assignment assignment, bool selected)
        {
            return new SelectListItem
            {
                Value = assignment.Id.ToString(CultureInfo.InvariantCulture),
                Text = assignment.FriendlyName,
                Selected = selected
            };
        }

        public Tournament ToTournament()
        {
            return new Tournament
            {
                Name = Name,
                CreateDateTime = DateTime.Now
            };
        }

       
    }
}