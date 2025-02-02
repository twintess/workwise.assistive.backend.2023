namespace workwise.assistive.backend.Model
{
    public class PopupDetailsResponse
    {
        public int EventId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Enabled { get; set; }
        public string CreatorFirstName { get; set; } = string.Empty;
        public string CreatorLastName { get; set;} = string.Empty;
        public string CreatorUsername { get; set;} = string.Empty;
        public string InsertDate { get; set; } = string.Empty;
    }
}