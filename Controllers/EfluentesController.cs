using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using ApiOracle.Models;
using ApiOracle.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiOracle.Controllers
{
    [ApiController]
    [Route("api/efluentes")]
    [Authorize]
    public class EfluentesController : ControllerBase
    {
        private readonly IEfluenteService _service;
        private readonly UsuarioService _usuarioService;
        private readonly ILogger<EfluentesController> _logger;
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public EfluentesController(IEfluenteService service, UsuarioService usuarioService, ILogger<EfluentesController> logger)
        {
            _service = service;
            _usuarioService = usuarioService;
            _logger = logger;
        }

        [HttpPost("init")]
        public async Task<IActionResult> CriarTabela()
        {
            await _service.CriarTabelaAsync();
            return Ok(new { message = "Tabelas PT_EFLUENTE e RT_EFLUENTE prontas" });
        }

        [HttpPost]
        [Consumes("application/json")]
        public async Task<ActionResult<PtEfluenteResponseDto>> Create([FromBody] PtEfluenteCreateDto dto)
        {
            try
            {
                var created = await _service.CriarAsync(dto, GetUsuarioId());
                return CreatedAtAction(nameof(Get), new { pk = created.PkCdMeioAmbienteCptm }, ToResponse(created));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("com-arquivo")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<PtEfluenteResponseDto>> CreateMultipart()
        {
            try
            {
                var dto = await ReadMultipartPayloadAsync<PtEfluenteCreateDto>();
                var created = await _service.CriarAsync(dto, GetUsuarioId());
                await AttachRequestFilesAsync(created.PkCdMeioAmbienteCptm);

                var response = ToResponse(created);
                response.Anexos = (await _service.ListarAnexosAsync(created.PkCdMeioAmbienteCptm)).Select(ToAttachmentResponse).ToList();

                return CreatedAtAction(nameof(Get), new { pk = created.PkCdMeioAmbienteCptm }, response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Listar(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? municipio = null,
            [FromQuery] string? linha = null,
            [FromQuery] string? status = null,
            [FromQuery] DateTime? data = null)
        {
            var normalizedPage = page < 1 ? 1 : page;
            var normalizedPageSize = pageSize switch
            {
                < 1 => 10,
                > 100 => 100,
                _ => pageSize
            };

            var requester = await GetRequesterAsync();
            if (requester == null) return Unauthorized();

            var items = (await _service.ListarPorUsuarioAsync(requester.Id, normalizedPage, normalizedPageSize, municipio, linha, status, data)).ToList();
            LogListResult("GET /api/efluentes", requester, requester.Id, normalizedPage, normalizedPageSize, municipio, linha, status, data, items.Count);
            return Ok(BuildListResponse(normalizedPage, normalizedPageSize, items));
        }

        [HttpGet("meus")]
        public async Task<IActionResult> ListarMeus(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? municipio = null,
            [FromQuery] string? linha = null,
            [FromQuery] string? status = null,
            [FromQuery] DateTime? data = null)
        {
            var requester = await GetRequesterAsync();
            if (requester == null) return Unauthorized();

            var normalizedPage = page < 1 ? 1 : page;
            var normalizedPageSize = pageSize switch
            {
                < 1 => 10,
                > 100 => 100,
                _ => pageSize
            };

            var items = (await _service.ListarPorUsuarioAsync(requester.Id, normalizedPage, normalizedPageSize, municipio, linha, status, data)).ToList();
            LogListResult("GET /api/efluentes/meus", requester, requester.Id, normalizedPage, normalizedPageSize, municipio, linha, status, data, items.Count);
            return Ok(BuildListResponse(normalizedPage, normalizedPageSize, items));
        }

        [HttpGet("/api/admin/usuarios/{usuarioId:int}/efluentes")]
        public async Task<IActionResult> ListarPorUsuarioAdmin(
            int usuarioId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? municipio = null,
            [FromQuery] string? linha = null,
            [FromQuery] string? status = null,
            [FromQuery] DateTime? data = null)
        {
            var requester = await GetRequesterAsync();
            if (requester == null) return Unauthorized();
            if (!requester.IsAdmin) return Forbid();

            var usuario = await _usuarioService.GetByIdAsync(usuarioId);
            if (usuario == null) return NotFound(new { message = "Usuario nao encontrado" });

            var normalizedPage = page < 1 ? 1 : page;
            var normalizedPageSize = pageSize switch
            {
                < 1 => 10,
                > 100 => 100,
                _ => pageSize
            };

            var items = (await _service.ListarPorUsuarioAsync(usuarioId, normalizedPage, normalizedPageSize, municipio, linha, status, data)).ToList();
            LogListResult("GET /api/admin/usuarios/{usuarioId}/efluentes", requester, usuarioId, normalizedPage, normalizedPageSize, municipio, linha, status, data, items.Count);
            return Ok(BuildListResponse(normalizedPage, normalizedPageSize, items));
        }

        [HttpGet("/api/admin/efluentes")]
        public async Task<IActionResult> ListarTodosAdmin(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? municipio = null,
            [FromQuery] string? linha = null,
            [FromQuery] string? status = null,
            [FromQuery] DateTime? data = null)
        {
            var requester = await GetRequesterAsync();
            if (requester == null) return Unauthorized();
            if (!requester.IsAdmin) return Forbid();

            var normalizedPage = page < 1 ? 1 : page;
            var normalizedPageSize = pageSize switch
            {
                < 1 => 10,
                > 100 => 100,
                _ => pageSize
            };

            var items = (await _service.ListarAdminAsync(normalizedPage, normalizedPageSize, municipio, linha, status, data)).ToList();
            LogListResult("GET /api/admin/efluentes", requester, null, normalizedPage, normalizedPageSize, municipio, linha, status, data, items.Count);
            return Ok(BuildListResponse(normalizedPage, normalizedPageSize, items));
        }

        [HttpGet("{pk}")]
        public async Task<ActionResult<PtEfluenteResponseDto>> Get(string pk)
        {
            try
            {
                var requester = await GetRequesterAsync();
                var item = await _service.GetByPkAsync(pk);
                _logger.LogInformation(
                    "Efluentes detalhe endpoint={Endpoint} usuarioLogado={UsuarioId} role={Role} pk={Pk} encontrado={Encontrado}",
                    "GET /api/efluentes/{pk}",
                    requester?.Id,
                    requester?.IsAdmin == true ? "Admin" : "User",
                    pk,
                    item != null);
                if (item == null) return NotFound(new { message = "Registro nao encontrado" });

                var response = ToResponse(item);
                response.Anexos = (await _service.ListarAnexosAsync(pk)).Select(ToAttachmentResponse).ToList();

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{pk}")]
        [Consumes("application/json")]
        public async Task<IActionResult> Update(string pk, [FromBody] PtEfluenteUpdateDto dto)
        {
            try
            {
                var updated = await _service.AtualizarAsync(pk, dto);
                if (!updated) return NotFound(new { message = "Registro nao encontrado" });

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{pk}/com-arquivo")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateMultipart(string pk)
        {
            try
            {
                var dto = await ReadMultipartPayloadAsync<PtEfluenteUpdateDto>();
                var updated = await _service.AtualizarAsync(pk, dto);
                if (!updated) return NotFound(new { message = "Registro nao encontrado" });

                await AttachRequestFilesAsync(pk);

                var item = await _service.GetByPkAsync(pk);
                if (item == null) return NotFound(new { message = "Registro nao encontrado" });

                var response = ToResponse(item);
                response.Anexos = (await _service.ListarAnexosAsync(pk)).Select(ToAttachmentResponse).ToList();

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{pk}")]
        public async Task<IActionResult> Delete(string pk)
        {
            try
            {
                var deleted = await _service.DeleteAsync(pk);
                if (!deleted) return NotFound(new { message = "Registro nao encontrado" });

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{pk}/anexos")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(50_000_000)]
        public async Task<IActionResult> UploadAnexos(string pk)
        {
            if (!Request.HasFormContentType)
                return BadRequest(new { message = "Envie multipart/form-data" });

            var form = await Request.ReadFormAsync();
            if (form.Files.Count == 0)
                return BadRequest(new { message = "Envie ao menos um arquivo" });

            var anexos = new List<RtEfluenteResponseDto>();
            try
            {
                foreach (var arquivo in form.Files)
                {
                    if (arquivo.Length <= 0)
                        return BadRequest(new { message = $"Arquivo vazio: {arquivo.FileName}" });

                    await using var ms = new MemoryStream();
                    await arquivo.CopyToAsync(ms);

                    var id = await _service.AnexarAsync(pk, ms.ToArray(), arquivo.ContentType, arquivo.FileName, arquivo.Length);
                    var anexo = await _service.ObterAnexoAsync(id);
                    if (anexo != null) anexos.Add(ToAttachmentResponse(anexo));
                }
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(anexos);
        }

        [HttpGet("{pk}/anexos")]
        public async Task<ActionResult<IEnumerable<RtEfluenteResponseDto>>> ListarAnexos(string pk)
        {
            try
            {
                var item = await _service.GetByPkAsync(pk);
                if (item == null) return NotFound(new { message = "Registro nao encontrado" });

                var anexos = await _service.ListarAnexosAsync(pk);
                return Ok(anexos.Select(ToAttachmentResponse));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("anexos/{attachmentId:int}")]
        public async Task<IActionResult> DownloadAnexo(int attachmentId)
        {
            var anexo = await _service.ObterAnexoAsync(attachmentId);
            if (anexo == null || anexo.Data.Length == 0)
                return NotFound(new { message = "Anexo nao encontrado" });

            var contentType = string.IsNullOrWhiteSpace(anexo.ContentType)
                ? "application/octet-stream"
                : anexo.ContentType;

            var fileName = string.IsNullOrWhiteSpace(anexo.AttName)
                ? $"anexo_{attachmentId}"
                : anexo.AttName;

            return File(anexo.Data, contentType, fileName);
        }

        private async Task<T> ReadMultipartPayloadAsync<T>()
        {
            if (!Request.HasFormContentType)
                throw new ArgumentException("Envie multipart/form-data");

            var form = await Request.ReadFormAsync();
            var payloadJson = form["payload"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(payloadJson))
                throw new ArgumentException("Campo payload obrigatorio");

            var dto = JsonSerializer.Deserialize<T>(payloadJson, JsonOptions);
            if (dto == null)
                throw new ArgumentException("Payload invalido");

            return dto;
        }

        private async Task AttachRequestFilesAsync(string pk)
        {
            var form = await Request.ReadFormAsync();
            if (form.Files.Count == 0) return;

            foreach (var arquivo in form.Files)
            {
                if (arquivo.Length <= 0)
                    throw new ArgumentException($"Arquivo vazio: {arquivo.FileName}");

                await using var ms = new MemoryStream();
                await arquivo.CopyToAsync(ms);

                await _service.AnexarAsync(pk, ms.ToArray(), arquivo.ContentType, arquivo.FileName, arquivo.Length);
            }
        }

        private int? GetUsuarioId()
        {
            var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                      ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(sub, out var usuarioId) ? usuarioId : null;
        }

        private async Task<Usuario?> GetRequesterAsync()
        {
            var usuarioId = GetUsuarioId();
            if (!usuarioId.HasValue) return null;

            return await _usuarioService.GetByIdAsync(usuarioId.Value);
        }

        private object BuildListResponse(int page, int pageSize, IEnumerable<PtEfluente> items) => new
        {
            page,
            pageSize,
            items = items.Select(ToResponse)
        };

        private void LogListResult(
            string endpoint,
            Usuario requester,
            int? usuarioFiltro,
            int page,
            int pageSize,
            string? municipio,
            string? linha,
            string? status,
            DateTime? data,
            int quantidade)
        {
            _logger.LogInformation(
                "Efluentes listagem endpoint={Endpoint} usuarioLogado={UsuarioId} role={Role} usuarioFiltro={UsuarioFiltro} page={Page} pageSize={PageSize} municipio={Municipio} linha={Linha} status={Status} data={Data} quantidade={Quantidade}",
                endpoint,
                requester.Id,
                requester.IsAdmin ? "Admin" : "User",
                usuarioFiltro,
                page,
                pageSize,
                municipio,
                linha,
                status,
                data?.ToString("yyyy-MM-dd"),
                quantidade);
        }

        private PtEfluenteResponseDto ToResponse(PtEfluente item) => new()
        {
            PkCdMeioAmbienteCptm = item.PkCdMeioAmbienteCptm,
            TxNrElementoMonitoramento = item.TxNrElementoMonitoramento,
            TxNmElementoMonitoramento = item.TxNmElementoMonitoramento,
            TxSiglaDeptoMeioAmbiente = item.TxSiglaDeptoMeioAmbiente,
            TxStatusDoDesvioAmbiental = item.TxStatusDoDesvioAmbiental,
            TxStatusDoRegistroNoBd = item.TxStatusDoRegistroNoBd,
            TxMunicipio = item.TxMunicipio,
            TxLinhaCptm = item.TxLinhaCptm,
            TxViaCptm = item.TxViaCptm,
            TxTrechoESentidoCptm = item.TxTrechoESentidoCptm,
            TxKmPoste = item.TxKmPoste,
            TxEstacaoCptm = item.TxEstacaoCptm,
            NrLatGrauDecimalWgs84 = item.NrLatGrauDecimalWgs84,
            NrLongGrauDecimalWgs84 = item.NrLongGrauDecimalWgs84,
            NrLatMetrosSirgas2000 = item.NrLatMetrosSirgas2000,
            NrLongMetrosSirgas2000 = item.NrLongMetrosSirgas2000,
            TxNmLocalEscopoContratual = item.TxNmLocalEscopoContratual,
            TxTipoDeFormulario = item.TxTipoDeFormulario,
            DtDataEmissaoFormulario = item.DtDataEmissaoFormulario,
            NrNumeroDeFormulario = item.NrNumeroDeFormulario,
            TxAutorPfDoFormulario = item.TxAutorPfDoFormulario,
            TxNaturezaDoPga = item.TxNaturezaDoPga,
            TxNomePjExecutora = item.TxNomePjExecutora,
            TxTipoAtividadeListada = item.TxTipoAtividadeListada,
            TxTipoAtividadeNListada = item.TxTipoAtividadeNListada,
            TxTipoDraListado = item.TxTipoDraListado,
            TxTipoDraNListado = item.TxTipoDraNListado,
            TxIdDra = item.TxIdDra,
            DtValidadeDra = item.DtValidadeDra,
            TxAnaliseCptmAprovacao = item.TxAnaliseCptmAprovacao,
            TxTipoAtividadeCptm = item.TxTipoAtividadeCptm,
            TxNmLocalAtiv = item.TxNmLocalAtiv,
            TxNmLocalAtivComplemento = item.TxNmLocalAtivComplemento,
            TxOrigemEfluente = item.TxOrigemEfluente,
            TxFonteGeradora = item.TxFonteGeradora,
            NrQuantidadeL = item.NrQuantidadeL,
            TxTipoDestinacao = item.TxTipoDestinacao,
            TxTipoVeiculo = item.TxTipoVeiculo,
            TxIdVeiculo = item.TxIdVeiculo,
            TxIdGuiaRemessa = item.TxIdGuiaRemessa,
            NrDistanciaDaViaM = item.NrDistanciaDaViaM,
            TxOfereceRiscoSistemaCptm = item.TxOfereceRiscoSistemaCptm,
            TxProprietario = item.TxProprietario,
            TxObsCadastramento = item.TxObsCadastramento,
            DtDataDoCadastramento = item.DtDataDoCadastramento,
            HrHoraDoCadastramento = item.HrHoraDoCadastramento,
            TxAutorPjDoCadastro = item.TxAutorPjDoCadastro,
            TxAutorPfDoCadastro = item.TxAutorPfDoCadastro,
            TxNmResponsavelCadastro = item.TxNmResponsavelCadastro,
            TxRpResponsavelCadastro = item.TxRpResponsavelCadastro,
            TxDrtResponsavelCadastro = item.TxDrtResponsavelCadastro,
            TxNomePjDaContratada = item.TxNomePjDaContratada,
            TxNrContratoContratada = item.TxNrContratoContratada,
            TxNmAreaGestoraCptm = item.TxNmAreaGestoraCptm,
            TxIdAreaGestoraCptm = item.TxIdAreaGestoraCptm,
            TxSiglaAreaGestoraCptm = item.TxSiglaAreaGestoraCptm,
            TxNomePfDaRepresentante = item.TxNomePfDaRepresentante,
            TxNomePjDaSupervisora = item.TxNomePjDaSupervisora,
            TxNrContratoSupervisora = item.TxNrContratoSupervisora,
            TxNmArquivoFdcRelacionado = item.TxNmArquivoFdcRelacionado,
            PkCdArquivoFdcRelacionado = item.PkCdArquivoFdcRelacionado,
            TxNmArquivoRvtRelacionado = item.TxNmArquivoRvtRelacionado,
            PkCdElementoDeMonitorRvt = item.PkCdElementoDeMonitorRvt,
            TxNmArquivoDacRelacionado = item.TxNmArquivoDacRelacionado,
            PkCdElementoDeMonitorDac = item.PkCdElementoDeMonitorDac,
            TxNmArquivoCncRelacionado = item.TxNmArquivoCncRelacionado,
            PkCdElementoDeMonitorCnc = item.PkCdElementoDeMonitorCnc,
            PkCdCodigoNoUltimoRra = item.PkCdCodigoNoUltimoRra,
            PkCdCedoc = item.PkCdCedoc,
            TxNomeFoto01 = item.TxNomeFoto01,
            TxNomeFoto02 = item.TxNomeFoto02,
            TxNomeFoto03 = item.TxNomeFoto03,
            TxNomeFoto04 = item.TxNomeFoto04,
            CreatedByUsuarioId = item.CreatedByUsuarioId,
            IsDeleted = item.IsDeleted,
            AttachmentCount = item.AttachmentCount,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };

        private RtEfluenteResponseDto ToAttachmentResponse(RtEfluente item) => new()
        {
            AttachmentId = item.AttachmentId,
            RelObjectId = item.RelObjectId,
            ContentType = item.ContentType,
            AttName = item.AttName,
            DataSize = item.DataSize,
            CreatedAt = item.CreatedAt,
            Url = Url.Action(nameof(DownloadAnexo), new { attachmentId = item.AttachmentId })
        };
    }
}
