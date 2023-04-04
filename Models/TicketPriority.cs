using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AtlasTracker.Models
{
    public class TicketPriority
    {
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        [DisplayName("Ticket Priority")]
        public string? Name { get; set; }


    }
}
