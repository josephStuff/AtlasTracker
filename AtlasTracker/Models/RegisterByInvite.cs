//using AtlasTracker.Services;
//using AtlasTracker.Services.Interfaces;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.UI.Services;
//using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Azure.Cosmos;
//using Microsoft.Azure.Documents;
//using System.ComponentModel;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.ComponentModel.Design;
//using System.Web.Mvc;
//using System.Web.Providers.Entities;
//using static Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal.ExternalLoginModel;
//using DataType = System.ComponentModel.DataAnnotations.DataType;
//using Index = System.Index;
//using User = Microsoft.Azure.Cosmos.User;

//namespace AtlasTracker.Models
//{
//    public class RegisterByInviteModel
//    {

//        private readonly SignInManager<BTUser> _signInManager;
//        private readonly UserManager<BTUser> _userManager;
//        private readonly IUserStore<BTUser> _userStore;
//        private readonly IUserEmailStore<BTUser> _emailStore;
//        private readonly ILogger<RegisterModel> _logger;
//        private readonly IEmailSender _emailSender;
//        private readonly IBTProjectService _projectService;
//        private readonly IBTInviteService _inviteService;

//        public RegisterByInviteModel(UserManager<BTUser> userManager,
//                                     IUserStore<BTUser> userStore,
//                                     SignInManager<BTUser> signInManager,
//                                     ILogger<RegisterModel> logger,
//                                     IEmailSender emailSender,
//                                     IBTProjectService projectService,
//                                     IBTInviteService inviteService,
//                                     IUserEmailStore<BTUser> emailStore)
//        {
//            _userManager = userManager;
//            _userStore = userStore;
//            _emailStore = emailStore;
//            _signInManager = signInManager;
//            _logger = logger;
//            _emailSender = emailSender;
//            _projectService = projectService;
//            _inviteService = inviteService;
//            _emailStore = emailStore;
//        }

//        [BindProperty]
//        public InputModel Input { get; set; }
        
        
//        public class InputModel 
//        {
//            [Required]
//            [StringLength(25, ErrorMessage = "The {0} must be at least {2} at most {1} characters long.", MinimumLength = 2)]
//            [DisplayName("First Name")]
//            public string FirstName { get; set; } = string.Empty;

//            [Required]
//            [StringLength(25, ErrorMessage = "The {0} must be at least {2} at most {1} characters long.", MinimumLength = 2)]
//            [DisplayName("Last Name")]
//            public string LastName { get; set; } = string.Empty;

//            [NotMapped]
//            [DataType(DataType.Upload)]
//            public string? FullName { get { return $"{FirstName} {LastName}"; } }

//            [Display(Name = "Company")]
//            public string? Company { get; set; }

//            [Required]
//            [DisplayName("Created Date")]
//            [DataType(DataType.Date)]
//            public int CompanyId { get; set; }

                                                                        
//            [DisplayName("Project Start Date")]
//            [DataType(DataType.Date)]
//            public DateTimeOffset StartDate { get; set; }


//            [DisplayName("Project End Date")]
//            [DataType(DataType.Date)]
//            public DateTimeOffset EndDate { get; set; }

//           public int ProjectPriorityId { get; set; }
//        }
    

//        public async Task OnGetAsync(int id, int companyId, string returnUrl = null)
//        {
//            ReturnUrl = returnUrl;
//            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

//            //Use "id" to find the invite  
//            Invite invite = await _inviteService.GetInviteAsync(id, companyId);

//            //Load Inputmodel with Invite information according to inviteId
//            Input.Email = invite.InviteeEmail;
//            Input.FirstName = invite.InviteeFirstName;
//            Input.LastName = invite.InviteeLastName;
//            Input.Company = invite.Company!.Name;
//            Input.CompanyId = invite.CompanyId;
//            Input.ProjectId = invite.ProjectId;
//        }


//        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
//        {
//            returnUrl ??= Url.Content("~/");
//            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

//            if (ModelState.IsValid)
//            {
//                var user = CreateUser(Input.CompanyId);

//                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
//                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
//                var result = await _userManager.CreateAsync(user, Input.Password);

//                if (result.Succeeded)
//                {
//                    _logger.LogInformation("User created a new account with password.");

//                    await _projectService.AddUserToProjectAsync(userId.Id, Input.ProjectId);

//                    await _userManager.AddToRoleAsync(user, nameof(BTRolesService.Submitter));

//                    //var userId = await;
//                    //var code = await;
//                    code = WebEncoders.Base64UrlEncode();
//                };

//                return RedirectToAction(nameof(Index));
//            }


//            ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsByCompanyAsync(companyId), "Id", "Description");
//            return View(invite);

//        }

//    }

//}
