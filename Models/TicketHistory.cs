﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AtlasTracker.Models
{
    public class TicketHistory
    {
        // ----------------- PRIMARY KEY -------------------- <
        public int Id { get; set; }

        
        [DisplayName("Updated Ticket Item")]
        public string? PropertyName { get; set; }


        [DisplayName("Description of Change")]
        public string? Description { get; set; }


        [DisplayName("Date Modified")]
        [DataType(DataType.Date)]
        public DateTimeOffset Created { get; set; }


        [DisplayName("Previous")]
        public string? OldValue { get; set; }


        [DisplayName("Current")]
        public string? NewValue { get; set; }


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
