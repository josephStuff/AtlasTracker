using AtlasTracker.Models;

namespace AtlasTracker.Services.Interfaces
{
    public interface IBTCompanyInfoService
    {

        public Task<Company> GetCompanyInfoByIdAsync(int? companyId);

        // --------------------------    ATLAS    ---------------------------- <

        public Task<List<BTUser>> GetAllMembersAsync(int? companyId);
        public Task<List<Project>> GetAllProjectsAsync(int? companyId);
        public Task<List<Ticket>> GetAllTicketsAsync(int? companyId);
        public Task<List<Invite>> GetAllInvitesAsync(int? companyId);


    }

}
