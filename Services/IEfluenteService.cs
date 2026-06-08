using ApiOracle.Models;

namespace ApiOracle.Services
{
    public interface IEfluenteService
    {
        Task CriarTabelaAsync();
        Task<PtEfluente> CriarAsync(PtEfluenteCreateDto dto, int? usuarioId);
        Task<bool> AtualizarAsync(string pk, PtEfluenteUpdateDto dto);
        Task<bool> DeleteAsync(string pk, int? deletedByUsuarioId);
        Task<bool> RestoreAsync(string pk);
        Task<PtEfluente?> GetByPkAsync(string pk);
        Task<IEnumerable<PtEfluente>> ListarAsync(int page, int pageSize, string? municipio, string? linha, string? status, DateTime? data);
        Task<IEnumerable<PtEfluente>> ListarPorUsuarioAsync(int usuarioId, int page, int pageSize, string? municipio, string? linha, string? status, DateTime? data);
        Task<IEnumerable<PtEfluente>> ListarAdminAsync(int page, int pageSize, string? municipio, string? linha, string? status, DateTime? data);
        Task<IEnumerable<PtEfluente>> ListarExcluidosAdminAsync(int page, int pageSize);
        Task<IEnumerable<PtEfluente>> ListarExcluidosPorUsuarioAsync(int usuarioId, int page, int pageSize);
        Task<int> AnexarAsync(string pk, byte[] data, string? contentType, string? attName, long dataSize);
        Task<IEnumerable<RtEfluente>> ListarAnexosAsync(string pk);
        Task<RtEfluente?> ObterAnexoAsync(int attachmentId);
    }
}
