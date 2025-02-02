using System.ComponentModel.DataAnnotations;

namespace workwise.assistive.backend.Model
{
    public record NewUserRequest()
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Firstname { get; set; } = string.Empty;
        [Required]
        public string Lastname { get; set; } = string.Empty;
        [Required]
        public List<string>? Roles { get; set; }
    }
}
