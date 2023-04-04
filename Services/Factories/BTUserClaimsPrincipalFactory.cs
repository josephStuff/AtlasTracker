﻿using AtlasTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace AtlasTracker.Services.Factories
{
    public class BTUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<BTUser, IdentityRole>
    {

        public BTUserClaimsPrincipalFactory(UserManager<BTUser> userManager,
                                                RoleManager<IdentityRole> rolemanager,
                                                IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, rolemanager, optionsAccessor)
        {

        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(BTUser user)
        {
            ClaimsIdentity identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim("CompanyId", user.CompanyId.ToString()));
            return identity;
        }

    }

}
