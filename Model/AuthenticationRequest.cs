using System.ComponentModel.DataAnnotations;

namespace workwise.assistive.backend.Model
{
    public class AuthenticationRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
