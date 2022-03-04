using AtlasTracker.Extensions;
using AtlasTracker.Models;
using AtlasTracker.Models.ViewModels;
using AtlasTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AtlasTracker.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserRolesController : Controller
    {
        private readonly BTRolesService _rolesService;
        private readonly BTCompanyInfoService _companyInfoService;

        public UserRolesController(BTRolesService rolesService, BTCompanyInfoService companyInfoService)
        {
            _rolesService = rolesService;
            _companyInfoService = companyInfoService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserRoles()
        {
            
            List<ManageUserRolesViewModel> model = new();

            int companyId = User.Identity!.GetCompanyId();

            List<BTUser> users = await _companyInfoService.GetAllMembersAsync(companyId);

            foreach (BTUser user in users)
            {
                ManageUserRolesViewModel viewModel = new();
                viewModel.BTUser = user;
                IEnumerable<string> selected = await _rolesService.GetUserRolesAsync(user);
                viewModel.Roles = new MultiSelectList(await _rolesService.GetRolesAsync(),"Name", "Name", selected);

                model.Add(viewModel);
            }


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryTokenAttribute]
        public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel member)
        {



            return View();
        }

    }

}
