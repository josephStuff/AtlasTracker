using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AtlasTracker.Models
{
    public class TicketComment
    {
        public int Id { get; set; }

        [Required]
        [StringLength(25, ErrorMessage = "The {0} must be at least {2} at most {1} characters long.", MinimumLength = 2)]
        [DisplayName("Member Comment")]        
        public string? Comment { get; set; }


        [DataType(DataType.Date)]
        [DisplayName("Date")]
        public DateTimeOffset Created { get; set; }

        public int TicketId { get; set; }

        [Required]
        public string? UserId { get; set; }


        // ------ NAVIGATION PROPERITES --------------------- <
        [DisplayName("Ticket")]
        public virtual Ticket? Ticket { get; set; }

        [DisplayName("Team Member")]
        public virtual BTUser? User { get; set; }




    }
}
