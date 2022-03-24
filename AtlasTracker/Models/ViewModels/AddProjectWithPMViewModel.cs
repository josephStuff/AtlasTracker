﻿using Microsoft.AspNetCore.Mvc.Rendering;

namespace AtlasTracker.Models.ViewModels
{
    public class AddProjectWithPMViewModel
    {

        public Project? Project { get; set; }

        public SelectList? PMList { get; set; }

        public string? PMID { get; set; }

        public SelectList? PriorityList { get; set; }

    }

}
