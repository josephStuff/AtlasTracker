﻿#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AtlasTracker.Data;
using AtlasTracker.Models;
using AtlasTracker.Extensions;
using AtlasTracker.Services.Interfaces;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;


namespace AtlasTracker.Controllers
{
    public class InvitesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTProjectService _projectService;
        private readonly IDataProtector _protector;
        private readonly IBTEmailService _emailService;
        private readonly IBTCompanyInfoService _companyInfoService;
        private readonly IBTInviteService _inviteService;
        private readonly UserManager<BTUser> _userManager;

        public InvitesController(ApplicationDbContext context, IBTProjectService projectService, IDataProtectionProvider dataProtectionProvider, 
                                                                IBTEmailService emailService, IBTCompanyInfoService companyInfoService, 
                                                                IBTInviteService inviteService, UserManager<BTUser> userManager)
        {
            _context = context;
            _projectService = projectService;
            _protector = dataProtectionProvider.CreateProtector("protectorKey");
            _emailService = emailService;
            _companyInfoService = companyInfoService;
            _inviteService = inviteService;
            _userManager = userManager;
        }

        // GET: Invites
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Invites.Include(i => i.Company).Include(i => i.Invitee).Include(i => i.Invitor).Include(i => i.Project);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Invites/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invite = await _context.Invites
                                                .Include(i => i.Company)
                                                .Include(i => i.Invitee)
                                                .Include(i => i.Invitor)
                                                .Include(i => i.Project)
                                                .FirstOrDefaultAsync(m => m.Id == id);

            if (invite == null)
            {
                return NotFound();
            }

            return View(invite);
        }

        // GET: Invites/Create
        public async Task<IActionResult> Create()
        {
            int companyId = User.Identity.GetCompanyId();

            ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsByCompanyAsync(companyId), "Id", "Description");
            return View();
        }

        // POST: Invites/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProjectId,InviteeEmail,InviteeFirstName,InviteeLastName,Message")] Invite invite)
        {
            int companyId = User.Identity.GetCompanyId();
            ModelState.Remove("InvitorId");

            if (ModelState.IsValid)
            {
                Guid guid = Guid.NewGuid();

                string token = _protector.Protect(guid.ToString());
                string email = _protector.Protect(invite.InviteeEmail);
                string company = _protector.Protect(companyId.ToString());

                string callbackUrl = Url.Action("ProcessInvite", "Invites", new { token, email, company }, protocol: Request.Scheme);

                string destination = invite.InviteeEmail;
                // Customize company name here --------------------------------- <
                Company btCompany = await _companyInfoService.GetCompanyInfoByIdAsync(companyId);
                string subject = $"Atlas BugTracker: {btCompany.Name} company Invite";

                await _emailService.SendEmailAsync(destination, subject, body);

                // Create record in the Invites table -------------------------- <
                invite.CompanyToken = guid;
                invite.CompanyId = companyId;
                invite.InviteDate = DateTimeOffset.Now;
                invite.InviteeId = _userManager.GetUserId(User);
                invite.IsValid = true;

                await _inviteService.AddNewInviteAsync(invite);


                return RedirectToAction(nameof(Index));
            }

            ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsByCompanyAsync(companyId), "Id", "Description");
            return View(invite);
        }

        [HttpGet]
        public async Task<IActionResult> ProcessInvite(string token, string email, string company)
        {
            if (token == null)
            {
                return NotFound();
            }

            Guid companyToken = Guid.Parse(_protector.Unprotect(token));
            string inviteeEmail = _protector.Unprotect(email);
            int companyId = int.Parse(_protector.Unprotect(company));

            try
            {
                Invite invite = await _inviteService.GetInviteAsync(companyToken, inviteeEmail, companyId);

                if (invite != null)
                {
                    return View(invite);
                }

                return NotFound();

            }

            catch (Exception)
            {
                throw;
            }
        }
    }
}
