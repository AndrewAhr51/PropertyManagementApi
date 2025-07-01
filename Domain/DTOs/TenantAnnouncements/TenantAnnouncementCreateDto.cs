namespace PropertyManagementAPI.Domain.DTOs.TenantAnnouncements
{
    public class TenantAnnouncementCreateDto
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string PostedBy { get; set; } = "System";
    }
}
