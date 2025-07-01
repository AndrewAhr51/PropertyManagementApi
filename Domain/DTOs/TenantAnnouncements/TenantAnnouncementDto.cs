namespace PropertyManagementAPI.Domain.DTOs.TenantAnnouncements
{
    public class TenantAnnouncementDto
    {
        public int AnnouncementId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string PostedBy { get; set; }
        public DateTime PostedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
