﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AtlasTracker.Models
{
    public class Ticket
    {
        // ------------------ PRIMARY KEY ---------------- <
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [DisplayName("Ticket Title")]
        public string? Title { get; set; }

        [Required]
        [StringLength(2000)]
        [DisplayName("Ticket Description")]
        public string? Description { get; set; }



        [DataType(DataType.Date)]
        [DisplayName("Created")]
        public DateTimeOffset? Created { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Updated")]
        public DateTimeOffset? Updated { get; set; }



        [DisplayName("Archived")]
        public bool Archived { get; set; }


        [DisplayName("Archived By Project")]
        public bool ArchivedByProject { get; set; }



        public int ProjectId { get; set; }

        public int TicketTypeId { get; set; }

        public int TicketPriorityId { get; set; }

        public int TicketStatusId { get; set; }


        [Required]
        public string? OwnerUserId { get; set; }

        public string? DeveloperUserId { get; set; }


        // --------------------- NAVIGATION PROPERTIES -------------------- <

        [DisplayName("Project")]
        public virtual Project? Project { get; set; }
        public virtual TicketPriority? TicketPriority { get; set; }

        public virtual TicketType? TicketType { get; set; }

        public virtual TicketStatus? TicketStatus { get; set; }

        public virtual BTUser? OwnerUser { get; set; }

        public virtual BTUser? DeveloperUser    { get; set; }

        public virtual ICollection<TicketComment> Comments { get; set; } = new HashSet<TicketComment>();
        public virtual ICollection<TicketAttachment> Attachments { get; set; } = new HashSet<TicketAttachment>();
        public virtual ICollection<TicketHistory> History { get; set; } = new HashSet<TicketHistory>();
        public virtual ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();



    }
}
