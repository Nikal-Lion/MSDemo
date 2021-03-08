using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;

namespace MS.Component.Jwt.UserClaim
{
    public class ClaimsAccessor : IClaimsAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClaimsAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal UserPrincipal
        {
            get
            {
                ClaimsPrincipal user = _httpContextAccessor.HttpContext.User;
                if (!user.Identity.IsAuthenticated)
                {
                    throw new Exception("用户未认证");
                }
                return user;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string UserName => UserPrincipal.Claims.First(x => x.Type == UserClaimType.Name).Value;
        /// <summary>
        /// 
        /// </summary>
        public long UserId => long.Parse(UserPrincipal.Claims.First(x => x.Type == UserClaimType.Id).Value);
        /// <summary>
        /// 
        /// </summary>
        public string UserAccount => UserPrincipal.Claims.First(x => x.Type == UserClaimType.Account).Value;
        /// <summary>
        /// 
        /// </summary>
        public string UserRole => UserPrincipal.Claims.First(x => x.Type == UserClaimType.RoleName).Value;
        /// <summary>
        /// 
        /// </summary>
        public string UserRoleDisplayName => UserPrincipal.Claims.First(x => x.Type == UserClaimType.RoleDisplayName).Value;
    }
}