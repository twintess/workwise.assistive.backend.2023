using System.ComponentModel.DataAnnotations;

namespace workwise.assistive.backend.Model
{
    public class PopupScheduleResponse
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Start { get; set; } = string.Empty;
        [Required]
        public string End { get; set; } = string.Empty;
        [Required]
        public bool Enabled { get; set; }
    }
}
