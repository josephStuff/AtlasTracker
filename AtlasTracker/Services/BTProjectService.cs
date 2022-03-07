using AtlasTracker.Data;
using AtlasTracker.Models;
using AtlasTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AtlasTracker.Services
{
    public class BTProjectService : IBTProjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTRolesService _rolesService;

        public BTProjectService(ApplicationDbContext context, IBTRolesService rolesService)
        {
            _context = context;
            _rolesService = rolesService;
        }

        public async Task AddNewProjectAsync(Project project)
        {
            try
            {
                _context.Add(project);
                await _context.SaveChangesAsync();
            }

            catch (Exception)
            {

                throw;
            }

        }

        public async Task<bool> AddProjectManagerAsync(string userId, int projectId)
        {
            BTUser currentPM = await GetProjectManagerAsync(projectId);

            // Remove the current PM if necessary
            if (currentPM != null)
            {
                try
                {
                    await RemoveProjectManagerAsync(projectId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error removing current PM. - Error: {ex.Message}");
                    return false;
                }
            }

            // Add the new PM
            try
            {
                await AddUserToProjectAsync(userId, projectId);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding new PM. - Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> AddUserToProjectAsync(string userId, int projectId)
        {
            BTUser? user = await _context.Users.FindAsync(userId);


            if (user != null)
            {
                Project? project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

                if (!await IsUserOnProjectAsync(userId, projectId))
                {
                    try
                    {
                        project.Members.Add(user);
                        await _context.SaveChangesAsync();
                        return true;
                    }
                    catch (Exception)
                    {

                        throw;
                    }

                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }

        }

        //  ----   CRUD --  (DELETE)  ARCHIVE   ------------------------------------------- <

        public async Task ArchiveProjectAsync(Project project)
        {
            try
            {
                project.Archived = true;
                await UpdateProjectAsync(project);

                foreach (Ticket ticket in project.Tickets)
                {
                    ticket.ArchivedByProject = true;
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
            }

            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<BTUser>> GetAllProjectMembersExceptPMAsync(int projectId)
        {
            try
            {
                List<BTUser> developers = await GetProjectMembersByRoleAsync(projectId, Roles.Developer.ToString());
                List<BTUser> submitters = await GetProjectMembersByRoleAsync(projectId, Roles.Submitter.ToString());
                List<BTUser> admins = await GetProjectMembersByRoleAsync(projectId, Roles.Admin.ToString());

                List<BTUser> teamMembers = developers.Concat(submitters).Concat(admins).ToList();

                return teamMembers;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Project>> GetAllProjectsByCompanyAsync(int companyId)
        {
            List<Project> projects = new();

            try
            {
                projects = await _context.Projects.Where(p => p.CompanyId == companyId && p.Archived == false)
                                                    .Include(p => p.Members)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.Comments)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.Attachments)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.History)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.Notifications)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.DeveloperUser)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.OwnerUser)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.TicketStatus)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.TicketPriority)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.TicketType)
                                                    .Include(p => p.ProjectPriority)
                                                    .ToListAsync();
                return projects;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Project>> GetAllProjectsByPriorityAsync(int companyId, string priorityName)
        {
            try
            {
                List<Project> projects = await GetAllProjectsByCompanyAsync(companyId);
                int priorityId = await LookupProjectPriorityId(priorityName);

                return projects.Where(p => p.ProjectPriorityId == priorityId).ToList();

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Project>> GetArchivedProjectsByCompanyAsync(int companyId)
        {
            try
            {
                List<Project> projects = await _context.Projects.Where(p => p.CompanyId == companyId && p.Archived == true)
                                                    .Include(p => p.Members)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.Comments)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.Attachments)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.History)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.Notifications)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.DeveloperUser)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.OwnerUser)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.TicketStatus)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.TicketPriority)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.TicketType)
                                                    .Include(p => p.ProjectPriority)
                                                    .ToListAsync();

                return projects;

            }
            catch (Exception)
            {

                throw;
            }
        }

        [Obsolete]
        public Task<List<BTUser>> GetDevelopersOnProjectAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task<Project> GetProjectByIdAsync(int projectId, int companyId)
        {
            try
            {
                //Project project = await _context.Projects
                //                        .Include(p => p.Tickets)
                //                        .Include(p => p.Members)
                //                        .Include(p => p.ProjectPriority)
                //                        .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == companyId);


                Project project = await _context.Projects.Include(p => p.Tickets).ThenInclude(t => t.TicketPriority)
                                                         .Include(p => p.Tickets).ThenInclude(t => t.TicketStatus)
                                                         .Include(p => p.Tickets).ThenInclude(t => t.TicketType)
                                                         .Include(p => p.Tickets).ThenInclude(t => t.DeveloperUser)
                                                         .Include(p => p.Tickets).ThenInclude(t => t.OwnerUser)
                                                         .Include(p => p.Members)
                                                         .Include(p => p.ProjectPriority)
                                                         .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == companyId);


                return project;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<BTUser> GetProjectManagerAsync(int projectId)
        {
            try
            {
                Project project = await _context.Projects
                                        .Include(p => p.Members)
                                        .FirstOrDefaultAsync(p => p.Id == projectId);

                foreach (BTUser member in project?.Members)
                {
                    if (await _rolesService.IsUserInRoleAsync(member, Roles.ProjectManager.ToString()))
                    {
                        return member;
                    }
                }

                return null;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<BTUser>> GetProjectMembersByRoleAsync(int projectId, string role)
        {
            try
            {
                List<BTUser> members = new();


                //  Get the Project and Include the members -------------------- >
                Project? project = await _context.Projects.Include(p => p.Members)
                                                          .FirstOrDefaultAsync(project => project.Id == projectId);


                foreach (BTUser user in project?.Members!)
                {
                    if (await _rolesService.IsUserInRoleAsync(user, role))
                    {
                        members.Add(user);
                    }
                }

                return members;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task<List<BTUser>> GetSubmittersOnProjectAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Project>> GetUnassignedProjectsAsync(int companyId)
        {
            List<Project> result = new();
            List<Project> projects = new();

            try
            {
                projects = await _context.Projects
                                         .Include(p => p.ProjectPriority)
                                         .Where(p => p.CompanyId == companyId).ToListAsync();

                foreach (Project project in projects)
                {
                    if ((await GetProjectMembersByRoleAsync(project.Id, nameof(Roles.ProjectManager))).Count == 0)
                    {
                        result.Add(project);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }

        public Task<List<Project>> GetUserProjectsAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<BTUser>> GetUsersNotOnProjectAsync(int projectId, int companyId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsAssignedProjectManagerAsync(string userId, int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsUserOnProjectAsync(string userId, int projectId)
        {
            try
            {
                Project project = await _context.Projects.Include(p => p.Members)
                                                         .FirstOrDefaultAsync(p => p.Id == projectId);

                bool result = false;

                if (project != null)
                {
                    result = project.Members.Any(m => m.Id == userId);
                }

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task<int> LookupProjectPriorityId(string priorityName)
        {
            throw new NotImplementedException();
        }

        public Task RemoveProjectManagerAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveUserFromProjectAsync(string userId, int projectId)
        {
            try
            {
                BTUser? user = await _context.Users.FindAsync(userId);
                Project? project = await _context.Projects.FindAsync(projectId);

                try
                {
                    if (await IsUserOnProjectAsync(userId, projectId))
                    {
                        project!.Members.Remove(user!);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"**** ERROR **** - Error Communicating with the DB. ----> {ex.Message}");
                    throw;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"**** ERROR **** - Error Removing User from project. ----> {ex.Message}");
                throw;
            }

        }

        public Task RemoveUsersFromProjectByRoleAsync(string role, int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task RestoreProjectAsync(Project project)
        {
            try
            {
                project.Archived = false;
                await UpdateProjectAsync(project);

                foreach (Ticket ticket in project.Tickets)
                {
                    ticket.ArchivedByProject = false;
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
            }

            catch (Exception)
            {

                throw;
            }
        }

        //  ----   CRUD --  UPDATE ------------------------------------------- <
        public async Task UpdateProjectAsync(Project project)
        {
            try
            {
                _context.Update(project);
                await _context.SaveChangesAsync();
            }

            catch (Exception)
            {

                throw;
            }
        }

    }

}
