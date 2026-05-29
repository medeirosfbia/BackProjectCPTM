using Microsoft.AspNetCore.Mvc;
using ApiOracle.Models;
using ApiOracle.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.StaticFiles;

namespace ApiOracle.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InspecoesController : ControllerBase
    {
        private readonly InspecaoService _service;
        private readonly UsuarioService _usuarioService;
        private static readonly FileExtensionContentTypeProvider _contentTypeProvider = new();

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
            public double? Latitude { get; set; }
            public double? Longitude { get; set; }
            public string Notes { get; set; }
            public string Q1 { get; set; }
            public string Q2 { get; set; }
            public string Q3 { get; set; }
            public string Q4 { get; set; }
            public string Q5 { get; set; }
            public string Q6 { get; set; }
        }

        public class UploadFotosDto
        {
            public List<IFormFile> Fotos { get; set; } = new();
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
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
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
                Latitude = i.Latitude,
                Longitude = i.Longitude,
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
        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<InspecaoDto>>> ListarPorUsuarioAdmin(int usuarioId)
        {
            var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                      ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(sub) || !int.TryParse(sub, out var requesterId))
                return Unauthorized();

            var requester = await _usuarioService.GetByIdAsync(requesterId);
            if (requester == null) return Unauthorized();

            if (!requester.IsAdmin) return Forbid();

            var usuario = await _usuarioService.GetByIdAsync(usuarioId);
            if (usuario == null) return NotFound(new { message = "Usuário não encontrado" });

            var items = await _service.ListarPorUsuarioAsync(usuarioId);

            var dto = items.Select(i => new InspecaoDto
            {
                Id = i.Id,
                Title = i.Title,
                Location = i.Location,
                Address = i.Address,
                Latitude = i.Latitude,
                Longitude = i.Longitude,
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
                Latitude = item.Latitude,
                Longitude = item.Longitude,
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

            var fotos = (await _service.ListarFotosAsync(id)).ToList();
            dto.Photos = fotos.Select(f => new InspecaoFotoDto
            {
                Id = f.Id,
                ContentType = f.ContentType,
                FileName = f.FileName,
                CreatedAt = f.CreatedAt,
                Url = Url.Action(nameof(GetFoto), values: new { id, fotoId = f.Id })!
            }).ToList();

            dto.ImagemUrl = Url.Action(nameof(GetImagem), values: new { id });

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
            existing.Latitude = dto.Latitude;
            existing.Longitude = dto.Longitude;
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

        [Authorize]
        [HttpPost("{id}/imagem")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(20_000_000)]
        public async Task<IActionResult> UploadImagem(int id, IFormFile imagem, [FromForm] List<IFormFile>? imagens)
        {
            var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(sub) || !int.TryParse(sub, out var requesterId))
                return Unauthorized();

            var requester = await _usuarioService.GetByIdAsync(requesterId);
            if (requester == null) return Unauthorized();

            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();
            if (!requester.IsAdmin && existing.UsuarioId != requester.Id) return Forbid();

            var arquivos = new List<IFormFile>();
            if (imagem != null && imagem.Length > 0) arquivos.Add(imagem);
            if (imagens != null && imagens.Count > 0) arquivos.AddRange(imagens.Where(f => f != null && f.Length > 0));

            if (arquivos.Count == 0)
                return BadRequest(new { message = "Arquivo de imagem é obrigatório" });

            foreach (var arquivo in arquivos)
            {
                if (arquivo.Length > 20_000_000)
                    return BadRequest(new { message = "Imagem excede o limite de 20MB" });

                var contentType = arquivo.ContentType;
                if (string.IsNullOrWhiteSpace(contentType) || !contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                    return BadRequest(new { message = "Content-Type inválido. Envie um arquivo de imagem." });

                await using var ms = new MemoryStream();
                await arquivo.CopyToAsync(ms);
                var bytes = ms.ToArray();

                await _service.AnexarFotoAsync(id, bytes, contentType, arquivo.FileName);
            }

            return NoContent();
        }

        [Authorize]
        [HttpGet("{id}/imagem")]
        public async Task<IActionResult> GetImagem(int id)
        {
            var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(sub) || !int.TryParse(sub, out var requesterId))
                return Unauthorized();

            var requester = await _usuarioService.GetByIdAsync(requesterId);
            if (requester == null) return Unauthorized();

            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();
            if (!requester.IsAdmin && existing.UsuarioId != requester.Id) return Forbid();

            var img = await _service.ObterImagemAsync(id);
            if (img != null)
            {
                var fileName = img.Value.FileName;
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    var fallbackExt = GetExtensionFromContentType(img.Value.ContentType);
                    fileName = $"inspecao_{id}{fallbackExt}";
                }

                return File(img.Value.Imagem, img.Value.ContentType, fileName);
            }

            var maisRecente = await _service.ObterFotoMaisRecenteAsync(id);
            if (maisRecente == null) return NotFound(new { message = "Inspeção não possui imagem" });

            var fileName2 = maisRecente.FileName;
            if (string.IsNullOrWhiteSpace(fileName2))
            {
                var fallbackExt = GetExtensionFromContentType(maisRecente.ContentType);
                fileName2 = $"inspecao_{id}_{maisRecente.Id}{fallbackExt}";
            }

            return File(maisRecente.Imagem, maisRecente.ContentType, fileName2);
        }

        [Authorize]
        [HttpPost("{id}/fotos")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(20_000_000)]
        public async Task<IActionResult> UploadFotos(int id, [FromForm] UploadFotosDto dto)
        {
            var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(sub) || !int.TryParse(sub, out var requesterId))
                return Unauthorized();

            var requester = await _usuarioService.GetByIdAsync(requesterId);
            if (requester == null) return Unauthorized();

            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();
            if (!requester.IsAdmin && existing.UsuarioId != requester.Id) return Forbid();

            if (dto?.Fotos == null || dto.Fotos.Count == 0)
                return BadRequest(new { message = "Envie ao menos 1 foto" });

            foreach (var arquivo in dto.Fotos.Where(f => f != null && f.Length > 0))
            {
                if (arquivo.Length > 20_000_000)
                    return BadRequest(new { message = "Imagem excede o limite de 20MB" });

                var contentType = arquivo.ContentType;
                if (string.IsNullOrWhiteSpace(contentType) || !contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                    return BadRequest(new { message = "Content-Type inválido. Envie um arquivo de imagem." });

                await using var ms = new MemoryStream();
                await arquivo.CopyToAsync(ms);
                await _service.AnexarFotoAsync(id, ms.ToArray(), contentType, arquivo.FileName);
            }

            return NoContent();
        }

        [Authorize]
        [HttpGet("{id}/fotos")]
        public async Task<ActionResult<IEnumerable<InspecaoFotoDto>>> ListarFotos(int id)
        {
            var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(sub) || !int.TryParse(sub, out var requesterId))
                return Unauthorized();

            var requester = await _usuarioService.GetByIdAsync(requesterId);
            if (requester == null) return Unauthorized();

            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();
            if (!requester.IsAdmin && existing.UsuarioId != requester.Id) return Forbid();

            var fotos = await _service.ListarFotosAsync(id);
            var dto = fotos.Select(f => new InspecaoFotoDto
            {
                Id = f.Id,
                ContentType = f.ContentType,
                FileName = f.FileName,
                CreatedAt = f.CreatedAt,
                Url = Url.Action(nameof(GetFoto), values: new { id, fotoId = f.Id })!
            });

            return Ok(dto);
        }

        [Authorize]
        [HttpGet("{id}/fotos/{fotoId}")]
        public async Task<IActionResult> GetFoto(int id, int fotoId)
        {
            var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(sub) || !int.TryParse(sub, out var requesterId))
                return Unauthorized();

            var requester = await _usuarioService.GetByIdAsync(requesterId);
            if (requester == null) return Unauthorized();

            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();
            if (!requester.IsAdmin && existing.UsuarioId != requester.Id) return Forbid();

            var foto = await _service.ObterFotoAsync(fotoId);
            if (foto == null || foto.InspecaoId != id) return NotFound();

            var fileName = foto.FileName;
            if (string.IsNullOrWhiteSpace(fileName))
            {
                var fallbackExt = GetExtensionFromContentType(foto.ContentType);
                fileName = $"inspecao_{id}_{fotoId}{fallbackExt}";
            }

            return File(foto.Imagem, foto.ContentType, fileName);
        }

        private static string GetExtensionFromContentType(string contentType)
        {
            if (string.IsNullOrWhiteSpace(contentType)) return ".bin";

            if (_contentTypeProvider.Mappings.Any(kvp => kvp.Value.Equals(contentType, StringComparison.OrdinalIgnoreCase)))
            {
                var ext = _contentTypeProvider.Mappings.First(kvp => kvp.Value.Equals(contentType, StringComparison.OrdinalIgnoreCase)).Key;
                return ext;
            }

            return contentType.ToLowerInvariant() switch
            {
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                "image/gif" => ".gif",
                "image/webp" => ".webp",
                _ => ".bin"
            };
        }
    }
}
