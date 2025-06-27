using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementAPI.Domain.Entities.User
{
    [Table("Emails")]
    public class Emails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmailId { get; set; }  // ✅ Unique Identifier

        [Required]
        [ForeignKey("Sender")]
        public string Sender { get; set; }  // ✅ FK to Users

        [Required]
        [MaxLength(255)]
        public string Recipient { get; set; }  // ✅ Email Address

        [Required]
        [MaxLength(255)]
        public string Subject { get; set; }  // ✅ Email Subject

        [Required]
        public string Body { get; set; }  // ✅ Email Content

        [Column(TypeName = "LONGBLOB")]
        public byte[] AttachmentBlob { get; set; }  // ✅ Stores email attachments as binary data

        [Column(TypeName = "datetime")]
        public DateTime SentDate { get; set; } = DateTime.UtcNow;  // ✅ Auto-populate timestamp

        public bool IsDelivered { get; set; } = false;  // ✅ Delivery status

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending";  // ✅ Tracks email status ('Pending', 'Sent', 'Failed')

    }
}