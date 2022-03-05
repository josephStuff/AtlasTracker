using AtlasTracker.Extensions;
using AtlasTracker.Models;
using AtlasTracker.Models.ViewModels;
using AtlasTracker.Services;
using AtlasTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AtlasTracker.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserRolesController : Controller
    {
        private readonly IBTRolesService _rolesService;
        private readonly IBTCompanyInfoService _companyInfoService;

        public UserRolesController(IBTRolesService rolesService, IBTCompanyInfoService companyInfoService)
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

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryTokenAttribute]
        public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel member)
        {
            // ------------------------------------------ GET THE COMPANY ID -------------------------------- >
            int companyId = User.Identity!.GetCompanyId();


            //   ---------------------------------------   INSTANTIATE THE BTUser ------------------------------ > 
            BTUser? btUser = (await _companyInfoService.GetAllMembersAsync(companyId)).FirstOrDefault(u => u.Id == member.BTUser?.Id);


            //   -------------------------------------------  GET selected ROLES FOR THE USER --------------------- >
            IEnumerable<string> roles = await _rolesService.GetUserRolesAsync(btUser!);


            //  ------------------------------------- remove user from their roles ---------------------- >
            string userRole = member.SelectedRoles?.FirstOrDefault()!;



            if (!string.IsNullOrEmpty(userRole))
            {

                //  --------------------- remove user from roles ----------------------- >
                if(await _rolesService.RemoveUserFromRolesAsync(btUser!, roles))
                {
                    // ---------------------- add user to the new role -----------------------  >
                    await _rolesService.AddUserToRoleAsync(btUser!, userRole);
                }

            }

            return RedirectToAction(nameof(ManageUserRoles));

        }

    }

}
