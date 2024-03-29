﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AtlasTracker.Models
{
    public class ProjectPriority
    {
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        [DisplayName("Project Name")]
        public string? Name { get; set; }


    }
}
