using MoCS.Data.Entity;
using System;
using System.ComponentModel.DataAnnotations;

namespace MoCS.Web.Models
{
    public class AssignmentModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Friendly name")]
        public string FriendlyName { get; set; }
        [Required]
        public string Tagline { get; set; }
        [Required]
        [Display(Name = "File path")]
        public string FilePath { get; set; }
        //public DateTime CreateDateTime { get; set; }

        public AssignmentModel() { }

        public AssignmentModel(Assignment assignment)
        {
            Id = assignment.Id;
            Name = assignment.Name;
            FriendlyName = assignment.FriendlyName;
            Tagline = assignment.Tagline;
            FilePath = assignment.FilePath;
        }

        public void UpdateAssignment(ref Assignment assignment)
        {
            Name = Name.Trim();
            FriendlyName = FriendlyName.Trim();
            Tagline = Tagline.Trim();
            FilePath = FilePath.Trim();

            var dirty = assignment.Name != Name
                    || assignment.FriendlyName != FriendlyName
                    || assignment.Tagline == Tagline
                    || assignment.FilePath == FilePath;

            assignment.Name = Name;
            assignment.FriendlyName = FriendlyName;
            assignment.Tagline = Tagline;
            assignment.FilePath = FilePath;

            if (dirty) assignment.LastModified = DateTime.UtcNow;
        }
    }
}