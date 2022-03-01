using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AtlasTracker.Models
{
    public class TicketComment
    {
        public int Id { get; set; }

        [Required]
        [StringLength(2000)]
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
