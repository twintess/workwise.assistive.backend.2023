using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace auth.mrds.net
{
    public static class AuthenticationProvider
    {
        public static ClaimsPrincipal PrepareClaimPrincipal(string authenticationScheme, string username, List<string> roles)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Sub, username),
            };

            roles.ForEach(role => claims.Add(new("role", role)));

            return new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationScheme));
        }
    }
}
