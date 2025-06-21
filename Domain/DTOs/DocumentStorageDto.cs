namespace PropertyManagementAPI.Domain.DTOs
{
    public class DocumentStorageDto
    {
        public int DocumentStorageId { get; set; }
        public int DocumentId { get; set; } // Foreign key reference to Documents
        public int DocumentTypeId { get; set; } // Foreign key reference to lkupDocumentType
        public string FileName { get; set; }
        public byte[] FileData { get; set; } // Blob storage
        public DateTime CreatedDate { get; set; }
    }
}
