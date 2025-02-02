using System.ComponentModel.DataAnnotations;

namespace workwise.assistive.backend.Model
{
    public class AuthenticationResponse
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public List<string>? Roles { get; set; }
    }
}