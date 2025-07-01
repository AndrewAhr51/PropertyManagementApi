namespace PropertyManagementAPI.Domain.DTOs.OwnerAnnouncements
{
    public class OwnerAnnouncementDto
    {
        public int AnnouncementId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string PostedBy { get; set; }
        public DateTime PostedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
