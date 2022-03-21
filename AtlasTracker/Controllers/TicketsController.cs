#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AtlasTracker.Data;
using AtlasTracker.Models;
using AtlasTracker.Services;
using AtlasTracker.Extensions;
using Microsoft.AspNetCore.Identity;
using AtlasTracker.Models.Enums;
using AtlasTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using AtlasTracker.Models.ViewModels;

namespace AtlasTracker.Controllers
{    
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTTicketService _ticketService;
        private readonly IBTCompanyInfoService _companyInfoService;
        private readonly IBTProjectService _projectService;
        private readonly IBTLookupService _lookupService;
        private readonly IBTFileService _fileService;
        private readonly IBTTicketHistoryService _ticketHistoryService;
        private readonly IBTNotificationService _notificationService;


        public TicketsController(ApplicationDbContext context, IBTTicketService ticketService, IBTCompanyInfoService companyInfoService,
                                                                UserManager<BTUser> userManager, IBTProjectService projectService,
                                                                IBTLookupService lookupService, IBTFileService fileService,
                                                                IBTTicketHistoryService ticketHistoryService, IBTNotificationService notificationService)
        {
            _context = context;
            _ticketService = ticketService;
            _companyInfoService = companyInfoService;
            _userManager = userManager;
            _projectService = projectService;
            _lookupService = lookupService;
            _fileService = fileService;
            _ticketHistoryService = ticketHistoryService;
            _notificationService = notificationService;
        }

        // get My Tickets -----------------------
        public async Task<IActionResult> MyTickets()
        {
            string userId = _userManager.GetUserId(User);
            int companyId = User.Identity.GetCompanyId();

            List<Ticket> tickets = await _ticketService.GetArchivedTicketsAsync(companyId);

            return View(tickets);
        }

        // get all tickets --------------------------------------------
        public async Task<IActionResult> AllTickets()
        {
            int companyId = User.Identity.GetCompanyId();
            List<Ticket> tickets = new();

            if (User.IsInRole(nameof(BTRole.Admin)) || User.IsInRole(nameof(BTRole.ProjectManager)))
            {
                tickets = await _companyInfoService.GetAllTicketsAsync(companyId);
            }
            else
            {
                tickets = (await _ticketService.GetAllTicketsByCompanyAsync(companyId)).Where(t => t.Archived).ToList();
            }

            return View(tickets);
        }

        //  get: archived tickets ============================
        public async Task<IActionResult> ArchivedTickets()
        {
            int companyId = User.Identity.GetCompanyId();
            List<Ticket> tickets = await _ticketService.GetArchivedTicketsAsync(companyId);

            return View(tickets);
        }

        // Get unassigned tickets ---------------------
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> UnassignedTickets()
        {
            int companyId = User.Identity.GetCompanyId();
            string btUserId = _userManager.GetUserId(User);

            List<Ticket> tickets = await _ticketService.GetUnassignedTicketsAsync(companyId);

            if (User.IsInRole(nameof(BTRole.Admin)))
            {
                return View(tickets);

            }
            else
            {
                List<Ticket> pmTickets = new();

                foreach (Ticket ticket in tickets)
                {
                    if (await _projectService.IsAssignedProjectManagerAsync(btUserId, ticket.ProjectId))
                    {
                        pmTickets.Add(ticket);
                    }
                }

                return View(pmTickets);

            }

        }


        // Get assign Developers ------------------------- <
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpGet]
        public async Task<IActionResult> AssignDeveloper(int id)
        {
            BTUser btUser = await _userManager.GetUserAsync(User);
            AssignDeveloperViewModel model = new();

            model.Ticket = await _ticketService.GetTicketByIdAsync(id);
            model.Developers = new SelectList(await _projectService.GetProjectMembersByRoleAsync(model.Ticket.ProjectId, nameof(BTRole.Developer)),
                                                "Id", "FullName");

            // Assign Developer Notification
            if (model.Ticket.DeveloperUserId != null)
            {
                Notification devNotification = new()
                {
                    TicketId = model.Ticket.Id,
                    NotificationTypeId = (await _lookupService.LookupNotificationTypeIdAsync(nameof(BTNotificationType.Ticket))).Value,
                    Title = "Ticket Updated",
                    Message = $"Ticket: {model.Ticket.Title}, was updated by {btUser.FullName}",
                    Created = DateTime.UtcNow,
                    SenderId = btUser.Id,
                    RecipientId = model.Ticket.DeveloperUserId
                };

                await _notificationService.AddNotificationAsync(devNotification);
                await _notificationService.SendEmailNotificationAsync(devNotification, "Ticket Updated");
            }

            return View(model); 
        }

        // Post Assign Developers -------------------------- <
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignDeveloper(AssignDeveloperViewModel model)
        {
            if (model.DeveloperId != null)
            {
                BTUser btUser = await _userManager.GetUserAsync(User);
                //oldTicket
                Ticket oldTicket = await _ticketService.GetTicketAsNoTrackingAsync(model.Ticket!.Id);

                try
                {
                    await _ticketService.AssignTicketAsync(model.Ticket.Id, model.DeveloperId);
                }
                catch (Exception)
                {

                    throw;
                }

                return RedirectToAction(nameof(Details), new { id = model.Ticket?.Id });
            }

            return RedirectToAction(nameof(AssignDeveloper), new { id = model.Ticket?.Id });

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTicketComment([Bind("Id, TicketId, Comment")] TicketComment ticketComment)
        {

            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                try
                {
                    ticketComment.UserId = _userManager.GetUserId(User);
                    ticketComment.Created = DateTime.UtcNow;

                    await _ticketService.AddTicketCommentAsync(ticketComment);

                    // Add History --------------------------------------- <
                    await _ticketHistoryService.AddHistoryAsync(ticketComment.TicketId, nameof(TicketComment), ticketComment.UserId);

                }
                catch (Exception)
                {

                    throw;
                }

            }

            return RedirectToAction("Details", new { id = ticketComment.TicketId });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTicketAttachment([Bind("Id, ImageFormFile, Description, TicketId")] TicketAttachment ticketAttachment)
        {
            string statusMessage;

            //ModelState.Remove("UserId");

            if (ModelState.IsValid && ticketAttachment.ImageFormFile != null)
            {
                ticketAttachment.ImageFileData = await _fileService.ConvertFileToByteArrayAsync(ticketAttachment.ImageFormFile);
                ticketAttachment.ImageFileName = ticketAttachment.ImageFormFile.FileName;
                ticketAttachment.ImageContentType = ticketAttachment.ImageFormFile.ContentType;

                ticketAttachment.Created = DateTimeOffset.Now;
                ticketAttachment.UserId = _userManager.GetUserId(User);

                await _ticketService.AddTicketAttachmentAsync(ticketAttachment);
                statusMessage = "Success: New attachment added to Ticket.";
            }
            else
            {
                statusMessage = "Error: Invalid data.";

            }

            return RedirectToAction("Details", new { id = ticketAttachment.TicketId, message = statusMessage });
        }

        
        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            int companyId = User.Identity.GetCompanyId();
            List<Ticket> tickets = await _ticketService.GetAllTicketsByCompanyAsync(companyId);
                
            return View(tickets);
            
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? ticketId)
        {
            if (ticketId == null)
            {
                return NotFound();
            }

            Ticket ticket = new();

            try
            {
               ticket = await _ticketService.GetTicketByIdAsync(ticketId.Value);

            }
            catch (Exception)
            {

                throw;
            }

            if (ticket == null)
            {
                return NotFound();
            }


            return View(ticket);
        }

        // GET: Tickets/Create
        public async Task<IActionResult> Create()
        {
            BTUser btUser = await _userManager.GetUserAsync(User);

            if (User.IsInRole(nameof(BTRole.Admin)))
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsByCompanyAsync(btUser.CompanyId), "Id", "Name");
            }
            else
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetUserProjectsAsync(btUser.Id), "Id", "Name");
            }

            ViewData["Projects"] = new SelectList(await _projectService.GetAllProjectsByCompanyAsync(btUser.CompanyId), "Id", "Name");
            ViewData["TicketPriorityId"] = new SelectList(await _lookupService.GetTicketPrioritiesAsync(), "Id", "Name");
            ViewData["TicketStatusId"] = new SelectList(await _lookupService.GetTicketStatusesAsync(), "Id", "Name");
            ViewData["TicketTypeId"] = new SelectList(await _lookupService.GetTicketTypesAsync(), "Id", "Name");

            return View();


        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Created,Updated,Archived,ArchivedByProject,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,DeveloperUserId")] Ticket ticket)
        {       
            BTUser btUser = await _userManager.GetUserAsync (User);
            ModelState.Remove("OwnerUserId");

            if (ModelState.IsValid)
            {
                try
                {
                    //ticket.Created = DateTime.UtcNow;
                    ticket.Created = DateTimeOffset.UtcNow;
                    ticket.OwnerUserId = btUser.Id;
                    //ticket.OwnerUserId = _userManager.GetUserId(User);
                    ticket.TicketStatusId = (await _ticketService.LookupTicketStatusIdAsync(nameof(BTTicketStatus.New))).Value;

                    await _ticketService.AddNewTicketAsync(ticket);

                }
                catch (Exception)
                {
                    throw;
                }

                return RedirectToAction(nameof(AllTickets));
                // we could instead redirect to the project details here -------------------------- < 
            }

            if (User.IsInRole(nameof(BTRole.Admin)))
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsByCompanyAsync(btUser.CompanyId), "Id", "Name");
            }
            else
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetUserProjectsAsync(btUser.Id), "Id", "Name");
            }

            ViewData["TicketPriorityId"] = new SelectList(await _lookupService.GetTicketTypesAsync(), "Id", "Name");
            ViewData["TicketTypeId"] = new SelectList(await _lookupService.GetTicketTypesAsync(), "Id", "Name");


            //: Ticket Create Notification
            BTUser projectManager = await _projectService.GetProjectManagerAsync(ticket.ProjectId);
            int companyId = User.Identity!.GetCompanyId()!;
            Notification notification = new()
            {
                TicketId = ticket.Id,
                Title = "New Ticket",
                Message = $"New Ticket: {ticket.Title}, was created by {btUser.FullName}",
                Created = DateTime.UtcNow,
                SenderId = btUser.Id,
                RecipientId = projectManager?.Id
            };
            if (projectManager != null)
            {
                await _notificationService.AddNotificationAsync(notification);
                await _notificationService.SendEmailNotificationAsync(notification, "New Ticket Added");
            }
            else
            {
                //Admin notification
                await _notificationService.AddNotificationAsync(notification);
                await _notificationService.SendEmailNotificationsByRoleAsync(notification, companyId, nameof(BTRole.Admin));
            }

            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ticket ticket = await _ticketService.GetTicketByIdAsync(id.Value);

            if (ticket == null)
            {
                return NotFound();
            }

            ViewData["TicketPriorityId"] = new SelectList(await _lookupService.GetTicketPrioritiesAsync(), "Id", "Name", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList(await _lookupService.GetTicketStatusesAsync(), "Id", "Name", ticket.TicketStatusId);
            ViewData["TicketTypeId"] = new SelectList(await _lookupService.GetTicketTypesAsync(), "Id", "Name", ticket.TicketTypeId);
            
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,ProjectId,TicketTypeId,TicketPriorityId")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                BTUser btUser = await _userManager.GetUserAsync(User);
                Ticket oldTicket = await _ticketService.GetTicketAsNoTrackingAsync(ticket.Id);

                try
                {

                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                Ticket newTicket = await _ticketService.GetTicketAsNoTrackingAsync(ticket.Id);
                await _ticketHistoryService.AddHistoryAsync(oldTicket, newTicket, btUser.Id);

                // Ticket Edit notification
                BTUser projectManager = await _projectService.GetProjectManagerAsync(ticket.ProjectId);
                int companyId = User.Identity!.GetCompanyId()!;
                Notification notification = new()
                {
                    TicketId = ticket.Id,
                    NotificationTypeId = (await _lookupService.LookupNotificationTypeIdAsync(nameof(BTNotificationType.Ticket))).Value,
                    Title = "Ticket updated",
                    Message = $"Ticket: {ticket.Title}, was updated by {btUser.FullName}",
                    Created = DateTime.UtcNow,
                    SenderId = btUser.Id,
                    RecipientId = projectManager?.Id
                };
                // Notify PM or Admin
                if (projectManager != null)
                {
                    await _notificationService.AddNotificationAsync(notification);
                    await _notificationService.SendEmailNotificationAsync(notification, "Ticket Updated");
                }
                else
                {
                    //Admin notification
                    await _notificationService.AddNotificationAsync(notification);
                    await _notificationService.SendEmailNotificationsByRoleAsync(notification, companyId, nameof(BTRole.Admin));
                }
                //Notify Developer
                if (ticket.DeveloperUserId != null)
                {
                    Notification devNotification = new()
                    {
                        TicketId = ticket.Id,
                        NotificationTypeId = (await _lookupService.LookupNotificationTypeIdAsync(nameof(BTNotificationType.Ticket))).Value,
                        Title = "Ticket Updated",
                        Message = $"Ticket: {ticket.Title}, was updated by {btUser.FullName}",
                        Created = DateTimeOffset.Now,
                        SenderId = btUser.Id,
                        RecipientId = ticket.DeveloperUserId
                    };
                    await _notificationService.AddNotificationAsync(devNotification);
                    await _notificationService.SendEmailNotificationAsync(devNotification, "Ticket Updated");
                }

                return RedirectToAction(nameof(AllTickets));
            }

            ViewData["TicketPriorityId"] = new SelectList(await _lookupService.GetProjectPrioritiesAsync(), "Id", "Name", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList(await _lookupService.GetTicketStatusesAsync(), "Id", "Name", ticket.TicketStatusId);
            ViewData["TicketTypeId"] = new SelectList(await _lookupService.GetTicketTypesAsync(), "Id", "Name", ticket.TicketTypeId);

            return RedirectToAction(nameof(Index));
        }


        // GET: Tickets/Archive/
        public async Task<IActionResult> Archive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int companyId = User.Identity.GetCompanyId();
            var ticket = _ticketService.GetTicketByIdAsync(companyId);


            return View(nameof(Archive));
        }

        // POST: Tickets/Archive/
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveConfirmed(int id)
        {
            int companyId = User.Identity.GetCompanyId();
            var ticket = await _ticketService.GetTicketByIdAsync(id);

            await _ticketService.ArchiveTicketAsync(ticket);


            return RedirectToAction(nameof(AllTickets));
        }


        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.DeveloperUser)
                .Include(t => t.OwnerUser)
                .Include(t => t.Project)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.Id == id);
        }

        public async Task<IActionResult> ShowFile(int id)
        {
            TicketAttachment ticketAttachment = await _ticketService.GetTicketAttachmentByIdAsync(id);
            string fileName = ticketAttachment.ImageFileName;
            byte[] fileData = ticketAttachment.ImageFileData;
            string ext = Path.GetExtension(fileName).Replace(".", "");

            Response.Headers.Add("Content-Disposition", $"inline; filename={fileName}");
            return File(fileData, $"application/{ext}");
        }
    }
}
