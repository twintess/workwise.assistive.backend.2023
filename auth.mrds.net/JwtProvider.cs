using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace auth.mrds.net
{
    public class JwtProvider
    {
        private readonly string _tokenKey;
        private readonly TimeSpan _tokenTimeout;
        private readonly string _issuer;
        private readonly string _audience;

        public JwtProvider(string tokenKey, TimeSpan tokenTimeout, string issuer, string audience)
        {
            _tokenKey = tokenKey;
            _tokenTimeout = tokenTimeout;
            _issuer = issuer;
            _audience = audience;
        }

        public string GenerateToken(string username, List<string> roles)
        {
            TimeSpan tokenLifetime = _tokenTimeout;
            var key = Encoding.UTF8.GetBytes(_tokenKey);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Sub, username),
            };

            roles.ForEach(role => claims.Add(new("role", role)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(tokenLifetime),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);
            return jwt;
        }

        public string GenerateToken(ClaimsIdentity identity)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.UtcNow.Add(_tokenTimeout),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenKey)), SecurityAlgorithms.HmacSha512)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);
            return jwt;
        }
    }
}
