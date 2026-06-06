namespace ApiOracle.Models
{
    public class RtEfluenteResponseDto
    {
        public int AttachmentId { get; set; }
        public string RelObjectId { get; set; } = string.Empty;
        public string? ContentType { get; set; }
        public string? AttName { get; set; }
        public long? DataSize { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Url { get; set; }
    }
}
