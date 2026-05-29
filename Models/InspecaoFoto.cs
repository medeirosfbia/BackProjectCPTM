using System;

namespace ApiOracle.Models
{
    public class InspecaoFoto
    {
        public int Id { get; set; }
        public int InspecaoId { get; set; }
        public byte[] Imagem { get; set; }
        public string ContentType { get; set; }
        public string? FileName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
