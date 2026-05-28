using System;

namespace ApiOracle.Models
{
    public class Inspecao
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public byte[]? Imagem { get; set; }
        public string? ImagemContentType { get; set; }
        public string? ImagemFileName { get; set; }
        public string Notes { get; set; }
        public string Q1 { get; set; }
        public string Q2 { get; set; }
        public string Q3 { get; set; }
        public string Q4 { get; set; }
        public string Q5 { get; set; }
        public string Q6 { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UsuarioId { get; set; }
    }
}
