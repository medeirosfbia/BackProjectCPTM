using System;

namespace ApiOracle.Models
{
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string NomeCompleto { get; set; }
        public string Email { get; set; }
        public DateTime DataNascimento { get; set; }
        public bool IsAdmin { get; set; }
    }
}
