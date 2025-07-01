using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.Entities.OwnerAnnouncements
{
    public class OwnerAnnouncement
    {
        [Key]
        public int AnnouncementId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime PostedDate { get; set; } = DateTime.UtcNow;
        public string PostedBy { get; set; } = "Web";
        public bool IsActive { get; set; } = true;
    }
}
