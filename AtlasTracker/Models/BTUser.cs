using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtlasTracker.Models
{
    public class BTUser : IdentityUser
    {
        [Required]
        [StringLength(25, ErrorMessage = "The {0} must be at least {2} at most {1} characters long.", MinimumLength = 2)]
        [DisplayName("First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(25, ErrorMessage = "The {0} must be at least {2} at most {1} characters long.", MinimumLength = 2)]
        [DisplayName("Last Name")]
        public string LastName { get; set; } = string.Empty;

        [NotMapped]
        [DataType(DataType.Upload)]
        public string? FullName { get { return $"{FirstName} {LastName}"; } }

        [DisplayName("Avatar")]
        public string? AvatarName { get; set; }
        public byte[]? AvatarData   { get; set; }
        
        [DisplayName("File Extension")]
        public string? AvatarContentType { get; set; }

        public int CompanyId { get; set; }

        public virtual Company? Company { get; set; }
        public virtual ICollection<Project>? Projects { get; set; }

    }
}
