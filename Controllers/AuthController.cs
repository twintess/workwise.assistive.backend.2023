using auth.mrds.net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using workwise.assistive.backend.Model;
using workwise.assistive.backend.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace workwise.assistive.backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly UserService _userService;

        public AuthController(ILogger<UserService> logger, UserService userService)
        {
            _logger = logger;
            _userService = userService;
        }


        [Route("login")]
        [HttpPost]
        public ActionResult Login([FromBody] AuthenticationRequest authenticationRequest)
        {
            var authenticationResponse = _userService.Authenticate(authenticationRequest);

            if(authenticationResponse != null && !string.IsNullOrEmpty(authenticationResponse.Username))
            {
                this.HttpContext.Session.Set("username", Encoding.UTF8.GetBytes(authenticationResponse.Username));

                var principal = AuthenticationProvider.PrepareClaimPrincipal(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    authenticationResponse.Username, authenticationResponse.Roles);

                this.HttpContext.SignInAsync(principal);
                return this.Ok(authenticationResponse);
            }
            else
            {
                return this.Unauthorized();
            }
        }

        [Route("logout")]
        [HttpGet]
        public IActionResult Logout()
        {
            this.HttpContext.Session.Clear();
            
            foreach(var cookie in HttpContext.Request.Cookies)
            {
                Response.Cookies.Delete(cookie.Key);
            }

            this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return this.SignOut();
        }

        [Route("token")]
        [HttpPost]
        public IActionResult RefreshToken()
        {
            try
            {
                var token = Request.Cookies["token"];
                var handler = new JwtSecurityTokenHandler();
                var readToken = handler.ReadJwtToken(token);

                var username = Encoding.UTF8.GetString(this.HttpContext.Session.Get("username"));

                if (!string.IsNullOrEmpty(username) && username == readToken.Subject)
                {
                    var roles = _userService.GetRoles(username);

                    if(roles is not null && roles.Count > 0)
                    {
                        var jwtProvider = new JwtProvider(Config.KEY,
                            TimeSpan.FromMinutes(Config.TOKEN_TIMEOUT),
                            Config.ISSUER,
                            Config.AUDIENCE);

                        var jwtToken = jwtProvider.GenerateToken(username, roles);

                        Response.Cookies.Append("token", jwtToken);

                        return this.Ok();
                    }
                    else
                    {
                        return this.Unauthorized();
                    }
                }
                else
                {
                    return this.Unauthorized();
                }
            }
            catch(Exception exc)
            {
                _logger.LogError(exc.ToString());
                return this.Unauthorized();
            }
        }

        [Route("register")]
        [HttpPost]
        public IActionResult RegisterUser([FromBody] NewUserRequest request)
        {
            var result = _userService.CreateUser(request);

            return result ? this.Created(new Uri(""), null) : this.Ok();
        }
    }
}
