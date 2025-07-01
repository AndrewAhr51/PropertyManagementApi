namespace PropertyManagementAPI.Domain.DTOs.OwnerAnnouncements
{
    public class OwnerAnnouncementCreateDto
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string PostedBy { get; set; } = "System";
    }
}
