//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MoCS.Data.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class Team
    {
        public Team()
        {
            this.AssignmentEnrollments = new HashSet<AssignmentEnrollment>();
            this.Submits = new HashSet<Submit>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public System.DateTime CreateDateTime { get; set; }
        public int Score { get; set; }
        public string AdminUser { get; set; }
    
        public virtual ICollection<AssignmentEnrollment> AssignmentEnrollments { get; set; }
        public virtual ICollection<Submit> Submits { get; set; }
    }
}
