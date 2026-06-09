using ApiOracle.Models;
using ApiOracle.Repositories;

namespace ApiOracle.Services
{
    public class UsuarioService
    {
        private readonly UsuarioRepository _repo;
        private const string DefaultAdminEmail = "admin@teste.com";
        private const string DefaultAdminPassword = "admin123";

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

        public async Task EnsureDefaultAdminAsync()
        {
            var existing = await _repo.GetByEmailAsync(DefaultAdminEmail);
            if (existing == null)
            {
                await RegistrarAsync(new Usuario
                {
                    NomeCompleto = "Administrador",
                    Email = DefaultAdminEmail,
                    DataNascimento = new DateTime(2000, 1, 1),
                    IsAdmin = true
                }, DefaultAdminPassword);

                return;
            }

            existing.NomeCompleto = string.IsNullOrWhiteSpace(existing.NomeCompleto)
                ? "Administrador"
                : existing.NomeCompleto.Trim();
            existing.Email = DefaultAdminEmail;
            existing.DataNascimento = existing.DataNascimento == default
                ? new DateTime(2000, 1, 1)
                : existing.DataNascimento;
            existing.IsAdmin = true;

            if (string.IsNullOrWhiteSpace(existing.PasswordHash) ||
                !PasswordHasher.Verify(existing.PasswordHash, DefaultAdminPassword))
            {
                existing.PasswordHash = PasswordHasher.Hash(DefaultAdminPassword);
            }

            await _repo.AtualizarAsync(existing);
        }

        public Task<IEnumerable<Usuario>> ListarAsync() => _repo.ListarAsync();

        public Task<Usuario?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);

        public async Task<Usuario?> AtualizarAsync(int id, string nomeCompleto, string email, DateTime dataNascimento, bool? isAdmin, string? senha)
        {
            if (string.IsNullOrWhiteSpace(nomeCompleto))
                throw new Exception("Nome completo Ã© obrigatÃ³rio");
            if (string.IsNullOrWhiteSpace(email))
                throw new Exception("Email Ã© obrigatÃ³rio");

            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;

            var emailOwner = await _repo.GetByEmailAsync(email);
            if (emailOwner != null && emailOwner.Id != id)
                throw new Exception("Email jÃ¡ cadastrado");

            if (existing.IsAdmin && isAdmin == false)
            {
                var totalAdmins = await _repo.ContarAdminsAsync();
                if (totalAdmins <= 1)
                    throw new InvalidOperationException("NÃ£o Ã© permitido remover o Ãºltimo administrador");
            }

            existing.NomeCompleto = nomeCompleto.Trim();
            existing.Email = email.Trim();
            existing.DataNascimento = dataNascimento;
            existing.IsAdmin = isAdmin ?? existing.IsAdmin;

            if (!string.IsNullOrWhiteSpace(senha))
                existing.PasswordHash = PasswordHasher.Hash(senha);

            var updated = await _repo.AtualizarAsync(existing);
            return updated ? existing : null;
        }

        public Task<bool> ExisteAdminAsync() => _repo.ExisteAdminAsync();

        public Task<int> ContarAdminsAsync() => _repo.ContarAdminsAsync();

        public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);
    }
}
