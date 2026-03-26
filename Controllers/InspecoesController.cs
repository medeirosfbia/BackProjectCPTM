using Microsoft.AspNetCore.Mvc;
using ApiOracle.Models;
using ApiOracle.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace ApiOracle.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InspecoesController : ControllerBase
    {
        private readonly InspecaoService _service;
        private readonly UsuarioService _usuarioService;

        public InspecoesController(InspecaoService service, UsuarioService usuarioService)
        {
            _service = service;
            _usuarioService = usuarioService;
        }

        [HttpPost("init")]
        public async Task<IActionResult> CriarTabela()
        {
            await _service.CriarTabelaAsync();
            return Ok("Tabela de inspeções pronta");
        }

        public class InspecaoCreateDto
        {
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
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InspecaoCreateDto dto)
        {
            var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(sub) || !int.TryParse(sub, out var usuarioId))
                return Unauthorized();

            var usuario = await _usuarioService.GetByIdAsync(usuarioId);
            if (usuario == null) return Unauthorized();

            var ins = new Inspecao
            {
                Title = dto.Title,
                Location = dto.Location,
                Address = dto.Address,
                Notes = dto.Notes,
                Q1 = dto.Q1,
                Q2 = dto.Q2,
                Q3 = dto.Q3,
                Q4 = dto.Q4,
                Q5 = dto.Q5,
                Q6 = dto.Q6,
                UsuarioId = usuario.Id
            };

            var id = await _service.CriarAsync(ins);
            return Ok(new { id });
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InspecaoDto>>> Listar()
        {
            var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(sub) || !int.TryParse(sub, out var requesterId))
                return Unauthorized();

            var requester = await _usuarioService.GetByIdAsync(requesterId);
            if (requester == null) return Unauthorized();

            IEnumerable<Inspecao> items;
            if (requester.IsAdmin)
            {
                items = await _service.ListarAsync();
            }
            else
            {
                items = await _service.ListarPorUsuarioAsync(requester.Id);
            }

            var dto = items.Select(i => new InspecaoDto
            {
                Id = i.Id,
                Title = i.Title,
                Location = i.Location,
                Address = i.Address,
                Notes = i.Notes,
                Q1 = i.Q1,
                Q2 = i.Q2,
                Q3 = i.Q3,
                Q4 = i.Q4,
                Q5 = i.Q5,
                Q6 = i.Q6,
                CreatedAt = i.CreatedAt,
                UsuarioId = i.UsuarioId
            });

            return Ok(dto);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<InspecaoDto>> Get(int id)
        {
            var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(sub) || !int.TryParse(sub, out var requesterId))
                return Unauthorized();

            var requester = await _usuarioService.GetByIdAsync(requesterId);
            if (requester == null) return Unauthorized();

            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            if (!requester.IsAdmin && item.UsuarioId != requester.Id) return Forbid();

            var dto = new InspecaoDto
            {
                Id = item.Id,
                Title = item.Title,
                Location = item.Location,
                Address = item.Address,
                Notes = item.Notes,
                Q1 = item.Q1,
                Q2 = item.Q2,
                Q3 = item.Q3,
                Q4 = item.Q4,
                Q5 = item.Q5,
                Q6 = item.Q6,
                CreatedAt = item.CreatedAt,
                UsuarioId = item.UsuarioId
            };

            return Ok(dto);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] InspecaoCreateDto dto)
        {
            var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(sub) || !int.TryParse(sub, out var requesterId))
                return Unauthorized();

            var requester = await _usuarioService.GetByIdAsync(requesterId);
            if (requester == null) return Unauthorized();

            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();
            if (!requester.IsAdmin && existing.UsuarioId != requester.Id) return Forbid();

            existing.Title = dto.Title;
            existing.Location = dto.Location;
            existing.Address = dto.Address;
            existing.Notes = dto.Notes;
            existing.Q1 = dto.Q1;
            existing.Q2 = dto.Q2;
            existing.Q3 = dto.Q3;
            existing.Q4 = dto.Q4;
            existing.Q5 = dto.Q5;
            existing.Q6 = dto.Q6;

            await _service.AtualizarAsync(existing);

            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(sub) || !int.TryParse(sub, out var requesterId))
                return Unauthorized();

            var requester = await _usuarioService.GetByIdAsync(requesterId);
            if (requester == null) return Unauthorized();

            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();
            if (!requester.IsAdmin && existing.UsuarioId != requester.Id) return Forbid();

            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
