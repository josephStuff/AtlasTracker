using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AtlasTracker.Models
{
    public class TicketPriority
    {
        public int Id { get; set; }

        [Required]
        [StringLength(25, ErrorMessage = "The {0} must be at least {2} at most {1} characters long.", MinimumLength = 2)]
        [DisplayName("Priority")]
        public string? Name { get; set; }
    }
}
