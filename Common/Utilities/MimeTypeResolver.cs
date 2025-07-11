namespace PropertyManagementAPI.Common.Utilities
{
    public static class MimeTypeResolver
    {
        private static readonly Dictionary<string, string> MimeTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            [".pdf"] = "application/pdf",
            [".doc"] = "application/msword",
            [".docx"] = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            [".xls"] = "application/vnd.ms-excel",
            [".xlsx"] = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            [".jpg"] = "image/jpeg",
            [".jpeg"] = "image/jpeg",
            [".png"] = "image/png",
            [".txt"] = "text/plain",
            [".csv"] = "text/csv",
            // Add more as needed
        };

        public static string GetMimeType(string fileName, ILogger? logger = null)
        {
            try
            {
                var ext = Path.GetExtension(fileName);
                if (string.IsNullOrWhiteSpace(ext))
                {
                    logger?.LogWarning("File extension missing or empty for: {FileName}", fileName);
                    return "application/octet-stream";
                }

                return MimeTypes.TryGetValue(ext, out var mime) ? mime : "application/octet-stream";
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Failed to resolve MIME type for: {FileName}", fileName);
                return "application/octet-stream";
            }
        }
    }
}
