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
    public class UsuariosController : ControllerBase
    {
        private readonly UsuarioService _service;
        private readonly TokenService _tokenService;

        public UsuariosController(UsuarioService service, TokenService tokenService)
        {
            _service = service;
            _tokenService = tokenService;
        }

        [HttpPost("init")]
        public async Task<IActionResult> CriarTabela()
        {
            await _service.CriarTabelaAsync();
            return Ok("Tabela de usuários pronta");
        }

        public class RegisterDto
        {
            public string NomeCompleto { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public DateTime DataNascimento { get; set; }
            public string Senha { get; set; } = string.Empty;
            public bool? IsAdmin { get; set; }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var isAdminToSet = dto.IsAdmin ?? false;

            if (User?.Identity?.IsAuthenticated == true)
            {
                var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(sub) || !int.TryParse(sub, out var requesterId))
                    return Unauthorized();

                var requester = await _service.GetByIdAsync(requesterId);
                if (requester == null)
                    return Unauthorized();
                if (!requester.IsAdmin)
                    return Forbid();
            }

            if (isAdminToSet)
            {
                if (User?.Identity?.IsAuthenticated != true)
                {
                    var hasAdmin = await _service.ExisteAdminAsync();
                    if (hasAdmin)
                        return Forbid();
                }
            }

            var user = new Usuario
            {
                NomeCompleto = dto.NomeCompleto,
                Email = dto.Email,
                DataNascimento = dto.DataNascimento,
                IsAdmin = isAdminToSet
            };

            var id = await _service.RegistrarAsync(user, dto.Senha);
            return Ok(new { id });
        }

        public class LoginDto
        {
            public string Email { get; set; } = string.Empty;
            public string Senha { get; set; } = string.Empty;
        }

        public class UsuarioUpdateDto
        {
            public string NomeCompleto { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public DateTime DataNascimento { get; set; }
            public bool? IsAdmin { get; set; }
            public string? Senha { get; set; }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _service.AuthenticateAsync(dto.Email, dto.Senha);
            if (user == null) return Unauthorized();

            var token = _tokenService.GenerateToken(user);
            return Ok(new { token });
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.UsuarioDto>>> Listar()
        {
            var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(sub) || !int.TryParse(sub, out var requesterId))
                return Unauthorized();

            var requester = await _service.GetByIdAsync(requesterId);
            if (requester == null) return Unauthorized();
            if (!requester.IsAdmin) return Forbid();

            var users = await _service.ListarAsync();
            var safe = users.Select(u => new Models.UsuarioDto
            {
                Id = u.Id,
                NomeCompleto = u.NomeCompleto,
                Email = u.Email,
                DataNascimento = u.DataNascimento,
                IsAdmin = u.IsAdmin
            });

            return Ok(safe);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Models.UsuarioDto>> GetById(int id)
        {
            var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(sub) || !int.TryParse(sub, out var requesterId))
                return Unauthorized();

            var requester = await _service.GetByIdAsync(requesterId);
            if (requester == null) return Unauthorized();
            if (!requester.IsAdmin && requester.Id != id) return Forbid();

            var user = await _service.GetByIdAsync(id);
            if (user == null) return NotFound();

            var safe = new Models.UsuarioDto
            {
                Id = user.Id,
                NomeCompleto = user.NomeCompleto,
                Email = user.Email,
                DataNascimento = user.DataNascimento,
                IsAdmin = user.IsAdmin
            };

            return Ok(safe);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<Models.UsuarioDto>> Update(int id, [FromBody] UsuarioUpdateDto dto)
        {
            var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(sub) || !int.TryParse(sub, out var requesterId))
                return Unauthorized();

            var requester = await _service.GetByIdAsync(requesterId);
            if (requester == null) return Unauthorized();
            if (!requester.IsAdmin) return Forbid();

            try
            {
                var updated = await _service.AtualizarAsync(
                    id,
                    dto.NomeCompleto,
                    dto.Email,
                    dto.DataNascimento,
                    dto.IsAdmin,
                    dto.Senha);

                if (updated == null) return NotFound();

                var safe = new Models.UsuarioDto
                {
                    Id = updated.Id,
                    NomeCompleto = updated.NomeCompleto,
                    Email = updated.Email,
                    DataNascimento = updated.DataNascimento,
                    IsAdmin = updated.IsAdmin
                };

                return Ok(safe);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(sub) || !int.TryParse(sub, out var requesterId))
                return Unauthorized();

            var requester = await _service.GetByIdAsync(requesterId);
            if (requester == null) return Unauthorized();
            if (!requester.IsAdmin && requester.Id != id) return Forbid();

            var target = await _service.GetByIdAsync(id);
            if (target == null) return NotFound();

            if (target.IsAdmin)
            {
                var totalAdmins = await _service.ContarAdminsAsync();
                if (totalAdmins <= 1)
                    return BadRequest(new { message = "Não é permitido excluir o último administrador" });
            }

            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
