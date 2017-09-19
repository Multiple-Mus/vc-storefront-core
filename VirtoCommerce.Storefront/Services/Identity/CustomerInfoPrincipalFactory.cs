﻿using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Customer;

namespace VirtoCommerce.Storefront.Services.Identity
{
    public class CustomerInfoPrincipalFactory : IUserClaimsPrincipalFactory<CustomerInfo>
    {
        private readonly IdentityOptions _options;

        public CustomerInfoPrincipalFactory(IOptions<IdentityOptions> optionsAccessor)
        {
            _options = optionsAccessor?.Value ?? new IdentityOptions();
        }

        public Task<ClaimsPrincipal> CreateAsync(CustomerInfo user)
        {
            //Create first anonymous identity
            var identity = new ClaimsIdentity();
            if(user.IsRegisteredUser)
            {
                //https://stackoverflow.com/questions/45261732/user-identity-isauthenticated-always-false-in-net-core-custom-authentication
                identity = new ClaimsIdentity("Registered");
            }

            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));

    
            if (!user.AllowedStores.IsNullOrEmpty())
            {
                identity.AddClaim(new Claim(StorefrontConstants.AllowedStoresClaimType, string.Join(",", user.AllowedStores)));
            }

            if (!string.IsNullOrEmpty(user.OperatorUserName))
            {
                identity.AddClaim(new Claim(StorefrontConstants.OperatorUserNameClaimType, user.OperatorUserName));
            }

            if (!string.IsNullOrEmpty(user.OperatorUserId))
            {
                identity.AddClaim(new Claim(StorefrontConstants.OperatorUserIdClaimType, user.OperatorUserId));
                identity.AddClaim(new Claim(StorefrontConstants.OperatorUserNameClaimType, user.OperatorUserName));
            }
            var principal = new ClaimsPrincipal(identity);

            return Task.FromResult(principal);
        }
    }
}