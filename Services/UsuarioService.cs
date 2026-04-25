using ApiOracle.Models;
using ApiOracle.Repositories;

namespace ApiOracle.Services
{
    public class UsuarioService
    {
        private readonly UsuarioRepository _repo;

        public UsuarioService(UsuarioRepository repo)
        {
            _repo = repo;
        }

        public Task CriarTabelaAsync() => _repo.CriarTabelaAsync();

        public async Task<int> RegistrarAsync(Usuario usuario, string senha)
        {
            if (string.IsNullOrWhiteSpace(usuario.NomeCompleto))
                throw new Exception("Nome completo é obrigatório");
            if (string.IsNullOrWhiteSpace(usuario.Email))
                throw new Exception("Email é obrigatório");

            var exists = await _repo.GetByEmailAsync(usuario.Email);
            if (exists != null)
                throw new Exception("Email já cadastrado");

            usuario.PasswordHash = PasswordHasher.Hash(senha);

            return await _repo.InserirAsync(usuario);
        }

        public async Task<Usuario?> AuthenticateAsync(string email, string senha)
        {
            var user = await _repo.GetByEmailAsync(email);
            if (user == null) return null;

            if (!PasswordHasher.Verify(user.PasswordHash, senha)) return null;

            return user;
        }

        public Task<IEnumerable<Usuario>> ListarAsync() => _repo.ListarAsync();

        public Task<Usuario?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);

        public Task<bool> ExisteAdminAsync() => _repo.ExisteAdminAsync();

        public Task<int> ContarAdminsAsync() => _repo.ContarAdminsAsync();

        public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);
    }
}
