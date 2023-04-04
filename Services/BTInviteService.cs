using AtlasTracker.Models;
using AtlasTracker.Services.Interfaces;

namespace AtlasTracker.Services
{
    public class BTInviteService : IBTInviteService
    {
        public Task<bool> AcceptInviteAsync(Guid? token, string userId, int companyId)
        {
            throw new NotImplementedException();
        }

        public async Task AddNewInviteAsync(Invite invite)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AnyInviteAsync(Guid token, string email, int companyId)
        {
            throw new NotImplementedException();
        }

        public async Task<Invite> GetInviteAsync(int inviteId, int companyId)
        {
            throw new NotImplementedException();
        }

        public async Task<Invite> GetInviteAsync(Guid token, string email, int companyId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ValidateInviteCodeAsync(Guid? token)
        {
            throw new NotImplementedException();
        }
    }
}
