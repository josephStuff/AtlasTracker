using AtlasTracker.Extensions;
using AtlasTracker.Models;
using AtlasTracker.Models.ViewModels;
using AtlasTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AtlasTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBTCompanyInfoService _companyInfoService;

        public HomeController(ILogger<HomeController> logger, IBTCompanyInfoService companyInfoService)
        {
            _logger = logger;
            _companyInfoService = companyInfoService;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Dashboard", "Home");
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        
        public async Task<IActionResult> Dashboard()
        {
            DashboardViewModel model = new();
            int companyId = User.Identity!.GetCompanyId();
            model.Company = await _companyInfoService.GetCompanyInfoByIdAsync(companyId);
            model.Projects = await _companyInfoService.GetAllProjectsAsync(companyId);
            model.Tickets = await _companyInfoService.GetAllTicketsAsync(companyId);
            model.Members = await _companyInfoService.GetAllMembersAsync(companyId);

            return View(model);
        }


    }
}