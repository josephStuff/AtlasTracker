using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtlasTracker.Models
{
    public class Project
    {
        // ------------- PRIMARY KEY ---------------------- <
        public int Id { get; set; }
                        
        public int CompanyId { get; set; }


        // ---------------- NAME ------------------------------- <
        [Required]
        [StringLength(25, ErrorMessage = "The {0} must be at least {2} at most {1} characters long.", MinimumLength = 2)]
        [DisplayName("Project Name")]
        public string? Name { get; set; }


        // ----------------- DESCRIPTION ------------------- <
        [Required]
        [StringLength(2000, ErrorMessage = "The {0} must be at least {2} at most {1} characters long.", MinimumLength = 2)]
        [DisplayName("Project Name")]
        public string? Description { get; set; }


        // -------------------- CREATED & DATES ----------------------------- <
        [Required]
        [DisplayName("Created Date")]
        [DataType(DataType.Date)]
        public DateTimeOffset CreatedDate { get; set; }

                
        [DisplayName("Project Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

                
        [DisplayName("Project End Date")]
        [DataType(DataType.Date)]
        public DateTimeOffset EndDate { get; set; }


        // ------------    IMAGES UPLOADS ----------------------------- <
        [NotMapped]
        [DataType(DataType.Upload)]
        public IFormFile? ImageFormFile { get; set; }


        [DisplayName("File Name")]
        public string? ImageFileName { get; set; } = "";


        public byte[]? ImageFileData { get; set; } = Array.Empty<byte>();


        [DisplayName("File Extension")]
        public string? ImageContentType { get; set; }

        public bool Archived { get; set; }



        
        // --------- NAVIGATION PROPERTIES ------------------------ <    
        
        public virtual Company? Company { get; set; }
        
        public virtual  ProjectPriority? ProjectPriority { get; set; }

        public virtual ICollection<BTUser> Members { get; set; } = new HashSet<BTUser>();

        public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();


    }
}
