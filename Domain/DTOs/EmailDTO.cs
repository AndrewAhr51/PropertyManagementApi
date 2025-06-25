using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.DTOs
{
    public class EmailDto
    {
        public int EmailId { get; set; }
        public string Sender { get; set; }
        public string EmailAddress { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public byte[] AttachmentBlob { get; set; } // Blob storage for attachments
        public DateTime SentDate { get; set; }
        public string Status { get; set; }
        public bool IsDelivered { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}