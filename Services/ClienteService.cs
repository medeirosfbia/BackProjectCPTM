using ApiOracle.Models;
using ApiOracle.Repositories;

namespace ApiOracle.Services
{
    public class ClienteService
    {
        private readonly ClienteRepository _repo;

        public ClienteService(ClienteRepository repo)
        {
            _repo = repo;
        }

        public Task CriarTabelaAsync() => _repo.CriarTabelaAsync();

        public Task<int> CriarClienteAsync(Cliente cliente)
        {
            if (string.IsNullOrWhiteSpace(cliente.Nome))
                throw new Exception("Nome é obrigatório");

            return _repo.InserirAsync(cliente);
        }

        public Task<IEnumerable<Cliente>> ListarAsync()
            => _repo.ListarAsync();
    }
}