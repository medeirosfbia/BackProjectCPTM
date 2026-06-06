using ApiOracle.Models;

namespace ApiOracle.Repositories
{
    public interface IEfluenteRepository
    {
        Task CriarTabelaAsync();
        Task<string> InserirAsync(PtEfluente efluente);
        Task<bool> AtualizarAsync(PtEfluente efluente);
        Task<bool> DeleteAsync(string pk);
        Task<PtEfluente?> GetByPkAsync(string pk);
        Task<IEnumerable<PtEfluente>> ListarAsync(int page, int pageSize, string? municipio, string? linha, string? status, DateTime? data);
        Task<IEnumerable<PtEfluente>> ListarPorUsuarioAsync(int usuarioId, int page, int pageSize, string? municipio, string? linha, string? status, DateTime? data);
        Task<IEnumerable<PtEfluente>> ListarAdminAsync(int page, int pageSize, string? municipio, string? linha, string? status, DateTime? data);
        Task<int> InserirAnexoAsync(string pk, byte[] data, string? contentType, string? attName, long dataSize);
        Task<IEnumerable<RtEfluente>> ListarAnexosAsync(string pk);
        Task<RtEfluente?> ObterAnexoAsync(int attachmentId);
    }
}
