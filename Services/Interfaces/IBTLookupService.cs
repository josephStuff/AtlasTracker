﻿using AtlasTracker.Models;

namespace AtlasTracker.Services
{
    public interface IBTLookupService
    {
        public Task<List<TicketPriority>> GetTicketPrioritiesAsync();

        public Task<List<TicketStatus>> GetTicketStatusesAsync();

        public Task<List<TicketType>> GetTicketTypesAsync();

        public Task<List<ProjectPriority>> GetProjectPrioritiesAsync();

        public Task<int?> LookupNotificationTypeIdAsync(string typeName);



    }

}
