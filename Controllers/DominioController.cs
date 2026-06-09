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
    [Route("api/dominios")]
    public class DominiosController : ControllerBase
    {
        private readonly DominioService _service;

        public DominiosController(DominioService service)
        {
            _service = service;
        }

        [HttpGet("siglas-departamento-meio-ambiente")]
        public async Task<IActionResult> ObterSiglasDeptoMeioAmbiente()
            => Ok(await _service.ObterSiglasDeptoMeioAmbienteAsync());

        [HttpGet("areas-gestoras")]
        public async Task<IActionResult> ObterAreasGestoras()
            => Ok(await _service.ObterAreasGestorasAsync());

        [HttpGet("diretorias-gerencias")]
        public async Task<IActionResult> ObterDiretoriasGerencias()
            => Ok(await _service.ObterDiretoriasGerenciasAsync());

        [HttpGet("naturezas-pga")]
        public async Task<IActionResult> ObterNaturezasPga()
            => Ok(await _service.ObterNaturezasPgaAsync());

        [HttpGet("status-desvio")]
        public async Task<IActionResult> ObterStatusDesvio()
            => Ok(await _service.ObterStatusDesvioAsync());

        [HttpGet("status-registro-bd")]
        public async Task<IActionResult> ObterStatusRegistroBd()
            => Ok(await _service.ObterStatusRegistroBdAsync());

        [HttpGet("municipios")]
        public async Task<IActionResult> ObterMunicipios()
            => Ok(await _service.ObterMunicipiosAsync());

        [HttpGet("linhas")]
        public async Task<IActionResult> ObterLinhas()
            => Ok(await _service.ObterLinhasAsync());

        [HttpGet("vias")]
        public async Task<IActionResult> ObterVias()
            => Ok(await _service.ObterViasAsync());

        [HttpGet("trechos-sentidos")]
        public async Task<IActionResult> ObterTrechosSentidos()
            => Ok(await _service.ObterTrechosSentidosAsync());

        [HttpGet("estacoes")]
        public async Task<IActionResult> ObterEstacoes()
            => Ok(await _service.ObterEstacoesAsync());

        [HttpGet("tipos-proprietario")]
        public async Task<IActionResult> ObterTiposProprietario()
            => Ok(await _service.ObterTiposProprietarioAsync());

        [HttpGet("tipos-proprietario-l13")]
        public async Task<IActionResult> ObterTiposProprietarioL13()
            => Ok(await _service.ObterTiposProprietarioL13Async());

        [HttpGet("proprietarios")]
        public async Task<IActionResult> ObterProprietarios()
            => Ok(await _service.ObterProprietariosAsync());

        [HttpGet("sim-nao")]
        public async Task<IActionResult> ObterSimNao()
            => Ok(await _service.ObterSimNaoAsync());

        [HttpGet("tipos-atividade-listada")]
        public async Task<IActionResult> ObterTiposAtividadeListada()
            => Ok(await _service.ObterTiposAtividadeListadaAsync());

        [HttpGet("tipos-dra-listado")]
        public async Task<IActionResult> ObterTiposDraListado()
            => Ok(await _service.ObterTiposDraListadoAsync());

        [HttpGet("tipos-atividade-cptm")]
        public async Task<IActionResult> ObterTiposAtividadeCptm()
            => Ok(await _service.ObterTiposAtividadeCptmAsync());

        [HttpGet("locais-atividade")]
        public async Task<IActionResult> ObterLocaisAtividade()
            => Ok(await _service.ObterLocaisAtividadeAsync());

        [HttpGet("origens-efluente")]
        public async Task<IActionResult> ObterOrigensEfluente()
            => Ok(await _service.ObterOrigensEfluenteAsync());

        [HttpGet("fontes-geradoras")]
        public async Task<IActionResult> ObterFontesGeradoras()
            => Ok(await _service.ObterFontesGeradorasAsync());

        [HttpGet("tipos-destinacao")]
        public async Task<IActionResult> ObterTiposDestinacao()
            => Ok(await _service.ObterTiposDestinacaoAsync());

        [HttpGet("tipos-veiculo")]
        public async Task<IActionResult> ObterTiposVeiculo()
            => Ok(await _service.ObterTiposVeiculoAsync());
    }
}