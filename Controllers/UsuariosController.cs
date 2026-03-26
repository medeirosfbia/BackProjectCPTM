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
            public string NomeCompleto { get; set; }
            public string Email { get; set; }
            public DateTime DataNascimento { get; set; }
            public string Senha { get; set; }
            // Optional admin flag. It will only be applied when the caller is
            // authenticated and already an admin. Otherwise it is ignored.
            public bool? IsAdmin { get; set; }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var isAdminToSet = false;

            if (User?.Identity?.IsAuthenticated == true)
            {
                var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(sub) && int.TryParse(sub, out var requesterId))
                {
                    var requester = await _service.GetByIdAsync(requesterId);
                    if (requester != null && requester.IsAdmin)
                    {
                        isAdminToSet = dto.IsAdmin ?? false;
                    }
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
            public string Email { get; set; }
            public string Senha { get; set; }
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
    }
}
