using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AtlasTracker.Models
{
    public class Company
    {
        // ---  PRIMARY KEY --------------------------------- <
        public int Id { get; set; } 

        [Required]
        [DisplayName("Company Name")]
        public string? Name { get; set; }

        [DisplayName("Company Description")]
        public string? Description { get; set; }

        // ------ NAVIGATION PROPERTIES ---------------------------------------- <
        public virtual ICollection<Project>? Projects { get; set; }
        public virtual ICollection<BTUser>? Members { get; set; }
        public virtual ICollection<Invite>? Invites { get; set; }
    }
}
