using auth.mrds.net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace workwise.assistive.backend
{
    public class JwtSecureDataFormat : ISecureDataFormat<AuthenticationTicket>
    {
        public string Protect(AuthenticationTicket data)
        {
            return string.Empty;
        }

        public string Protect(AuthenticationTicket data, string? purpose)
        {
            try
            {
                var identity = data.Principal.Identity as ClaimsIdentity;

                if (identity is not null)
                {
                    var jwtProvider = new JwtProvider(Config.KEY,
                        TimeSpan.FromMinutes(Config.TOKEN_TIMEOUT),
                        Config.ISSUER,
                        Config.AUDIENCE);

                    var jwtToken = jwtProvider.GenerateToken(identity);
                    return jwtToken;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public AuthenticationTicket? Unprotect(string? protectedText)
        {
            if (string.IsNullOrEmpty(protectedText)) throw new ArgumentNullException(nameof(protectedText));

            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = Config.ISSUER,
                    ValidAudience = Config.AUDIENCE,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config.KEY)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    AlgorithmValidator = (algorithm, key, token, parameters) => algorithm == SecurityAlgorithms.HmacSha512,
                    RequireExpirationTime = true,
                    RequireAudience = true,
                    RequireSignedTokens = true
                };

                var handler = new JwtSecurityTokenHandler();

                var readToken = handler.ReadJwtToken(protectedText);


                var principal = handler.ValidateToken(readToken.RawData, tokenValidationParameters, out SecurityToken token);

                return new AuthenticationTicket(principal, CookieAuthenticationDefaults.AuthenticationScheme);
            }
            catch
            {
                return null;
            }
        }

        public AuthenticationTicket? Unprotect(string? protectedText, string? purpose)
        {
            if(string.IsNullOrEmpty(protectedText)) throw new ArgumentNullException(nameof(protectedText));

            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = Config.ISSUER,
                    ValidAudience = Config.AUDIENCE,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config.KEY)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    RequireExpirationTime = true,
                    RequireAudience = true,
                    RequireSignedTokens = true,
                    AlgorithmValidator = (algorithm, key, token, parameters) => algorithm == SecurityAlgorithms.HmacSha512,
                    LifetimeValidator = (from, to, token, parameters) =>
                    {
                        return to > DateTime.UtcNow;
                    },
                };

                var handler = new JwtSecurityTokenHandler();

                var readToken = handler.ReadJwtToken(protectedText);

                var principal = handler.ValidateToken(readToken.RawData, tokenValidationParameters, out SecurityToken token);
                return new AuthenticationTicket(principal, CookieAuthenticationDefaults.AuthenticationScheme);
            }
            catch
            {
                return null;
            }
        }
    }
}
