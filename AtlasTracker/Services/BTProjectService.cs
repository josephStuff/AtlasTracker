using AtlasTracker.Data;
using AtlasTracker.Models;
using AtlasTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AtlasTracker.Services
{
    public class BTProjectService : IBTProjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<BTUser> _userManager;

        public BTProjectService(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, 
                                                                UserManager<BTUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }


        public async Task AddNewProjectAsync(Project project)
        {
            try
            {

            }

            catch (Exception)
            {

                throw;
            }

        }

        
        public Task<bool> AddProjectManagerAsync(string userId, int projectId)
        {
            try
            {
                
            }

            catch (Exception)
            {

                throw;
            }

        }

        public Task<bool> AddUserToProjectAsync(string userId, int projectId)
        {
            try
            {
                bool user = await _userManager.GetUserIdAsync(userId, projectId);
            }

            catch (Exception)
            {

                throw;
            }

        }

        public async Task ArchiveProjectAsync(Project project)
        {
            try
            {

            }

            catch (Exception)
            {

                throw;
            }

        }

        public async Task<List<Project>> GetAllProjectsByCompany(int companyId)
        {
            try
            {
                
            }

            catch (Exception)
            {

                throw;
            }

        }

        public async Task<List<Project>> GetAllProjectsByPriority(int companyId, string priorityName)
        {
            try
            {

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
                List<BTUser> bTUsers = new List<BTUser>();
                return bTUsers;
            }

            catch (Exception)
            {

                throw;
            }

        }

        public async Task<List<Project>> GetArchivedProjectsByCompany(int companyId)
        {
            try
            {
                //List<Project> projects =
            }

            catch (Exception)
            {

                throw;
            }

        }

        public async Task<List<BTUser>> GetDevelopersOnProjectAsync(int projectId)
        {
            try
            {

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

            }

            catch (Exception)
            {

                throw;
            }

        }

        public async Task<Project> GetProjectByIdAsync(int projectId, int companyId)
        {
            try
            {

            }

            catch (Exception)
            {

                throw;
            }

        }

        public async Task<List<BTUser>> GetSubmittersOnProjectAsync(int projectId)
        {
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<BTUser>> GetUsersNotOnProjectAsync(int projectId, int companyId)
        {
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Project>> GetUserProjectsAsync(string userId)
        {
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task<bool> IsUserOnProject(string userId, int projectId)
        {
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<int> LookupProjectPriorityId(string priorityName)
        {
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task RemoveProjectManagerAsync(int projectId)
        {
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task RemoveUsersFromProjectByRoleAsync(string role, int projectId)
        {
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task RemoveUserFromProjectAsync(string userId, int projectId)
        {
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task UpdateProjectAsync(Project project)
        {
            try
            {

            }
            catch (Exception)
            {

                throw;
            }

        }

    }

}
