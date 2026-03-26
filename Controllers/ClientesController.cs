using Microsoft.AspNetCore.Mvc;
using ApiOracle.Models;
using ApiOracle.Services;

namespace ApiOracle.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly ClienteService _service;

        public ClientesController(ClienteService service)
        {
            _service = service;
        }

        [HttpPost("init")]
        public async Task<IActionResult> CriarTabela()
        {
            await _service.CriarTabelaAsync();
            return Ok("Tabela pronta");
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] Cliente cliente)
        {
            var id = await _service.CriarClienteAsync(cliente);
            return Ok(new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var clientes = await _service.ListarAsync();
            return Ok(clientes);
        }
    }
}