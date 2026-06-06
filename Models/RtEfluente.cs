namespace ApiOracle.Models
{
    public class RtEfluente
    {
        public int AttachmentId { get; set; }
        public string RelObjectId { get; set; } = string.Empty;
        public string? ContentType { get; set; }
        public string? AttName { get; set; }
        public long? DataSize { get; set; }
        public byte[] Data { get; set; } = Array.Empty<byte>();
        public DateTime CreatedAt { get; set; }
    }
}
