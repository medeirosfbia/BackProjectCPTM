using ApiOracle.Models;
using ApiOracle.Repositories;

namespace ApiOracle.Services
{
    public class InspecaoService
    {
        private readonly InspecaoRepository _repo;

        public InspecaoService(InspecaoRepository repo)
        {
            _repo = repo;
        }

        public Task CriarTabelaAsync() => _repo.CriarTabelaAsync();

        public async Task<int> CriarAsync(Inspecao inspecao)
        {
            if (string.IsNullOrWhiteSpace(inspecao.Title))
                throw new Exception("Title é obrigatório");

            if (inspecao.UsuarioId <= 0)
                throw new Exception("Inspeção deve estar relacionada a um usuário");

            if (inspecao.CreatedAt == default(DateTime))
                inspecao.CreatedAt = DateTime.UtcNow;

            return await _repo.InserirAsync(inspecao);
        }

        public Task AtualizarAsync(Inspecao inspecao) => _repo.AtualizarAsync(inspecao);

        public Task<Inspecao?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);

        public Task<IEnumerable<Inspecao>> ListarAsync() => _repo.ListarAsync();

        public Task<IEnumerable<Inspecao>> ListarPorUsuarioAsync(int usuarioId) => _repo.ListarPorUsuarioAsync(usuarioId);

        public Task DeleteAsync(int id) => _repo.DeleteAsync(id);

        public Task AtualizarImagemAsync(int inspecaoId, byte[] imagem, string contentType, string? fileName) =>
            _repo.AtualizarImagemAsync(inspecaoId, imagem, contentType, fileName);

        public Task<(byte[] Imagem, string ContentType, string? FileName)?> ObterImagemAsync(int inspecaoId) =>
            _repo.ObterImagemAsync(inspecaoId);
    }
}
