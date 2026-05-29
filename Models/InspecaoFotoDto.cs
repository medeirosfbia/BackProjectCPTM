using System;

namespace ApiOracle.Models
{
    public class InspecaoFotoDto
    {
        public int Id { get; set; }
        public string ContentType { get; set; }
        public string? FileName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Url { get; set; }
    }
}
