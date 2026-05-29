using System;
using System.Collections.Generic;

namespace ApiOracle.Models
{
    public class InspecaoDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string Address { get; set; }
        public string Notes { get; set; }
        public string Q1 { get; set; }
        public string Q2 { get; set; }
        public string Q3 { get; set; }
        public string Q4 { get; set; }
        public string Q5 { get; set; }
        public string Q6 { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UsuarioId { get; set; }
        public double? Latitude { get; internal set; }
        public double? Longitude { get; internal set; }

        public string? ImagemUrl { get; set; }
        public List<InspecaoFotoDto>? Photos { get; set; }
    }
}
