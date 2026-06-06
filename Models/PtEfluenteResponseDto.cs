namespace ApiOracle.Models
{
    public class PtEfluenteResponseDto : PtEfluente
    {
        public List<RtEfluenteResponseDto> Anexos { get; set; } = new();
    }
}
