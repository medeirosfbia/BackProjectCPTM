using ApiOracle.Models;
using ApiOracle.Repositories;

namespace ApiOracle.Services
{
    public class DominioService
    {
        private readonly DominioRepository _repository;

        public DominioService(DominioRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<DominioItem>> ObterSiglasDeptoMeioAmbienteAsync()
            => _repository.ListarItensAsync("GEA_TX_SIGLA_DEPTO_MEIO_AMBIENTE");

        public Task<IEnumerable<DominioItem>> ObterAreasGestorasAsync()
            => _repository.ListarItensAsync("GEA_TX_NM_AREA_GESTORA_CPTM");

        public Task<IEnumerable<DominioItem>> ObterDiretoriasGerenciasAsync()
            => _repository.ListarItensAsync("GEA_DIR_GER_DEPTO_CPTM");

        public Task<IEnumerable<DominioItem>> ObterNaturezasPgaAsync()
            => _repository.ListarItensAsync("GEA_TX_NATUREZA_DO_PGA");

        public Task<IEnumerable<DominioItem>> ObterStatusDesvioAsync()
            => _repository.ListarItensAsync("GEA_TX_STATUS_DO_DESVIO_AMBIENTAL");

        public Task<IEnumerable<DominioItem>> ObterStatusRegistroBdAsync()
            => _repository.ListarItensAsync("GEA_TX_STATUS_DO_REGISTRO_NO_BD");

        public Task<IEnumerable<DominioItem>> ObterMunicipiosAsync()
            => _repository.ListarItensAsync("GEA_TX_MUNICIPIO");

        public Task<IEnumerable<DominioItem>> ObterLinhasAsync()
            => _repository.ListarItensAsync("GEA_TX_LINHA_CPTM");

        public Task<IEnumerable<DominioItem>> ObterViasAsync()
            => _repository.ListarItensAsync("GEA_TX_VIA_CPTM");

        public Task<IEnumerable<DominioItem>> ObterTrechosSentidosAsync()
            => _repository.ListarItensAsync("GEA_TX_TRECHO_E_SENTIDO_CPTM");

        public Task<IEnumerable<DominioItem>> ObterEstacoesAsync()
            => _repository.ListarItensAsync("GEA_TX_ESTACAO_CPTM");

        public Task<IEnumerable<DominioItem>> ObterTiposProprietarioAsync()
            => _repository.ListarItensAsync("TIPO_PROPRIETARIO");

        public Task<IEnumerable<DominioItem>> ObterTiposProprietarioL13Async()
            => _repository.ListarItensAsync("TIPO_PROPRIETARIO_L13");

        public Task<IEnumerable<DominioItem>> ObterProprietariosAsync()
            => _repository.ListarItensAsync("GEA_TX_PROPRIETARIO");

        public Task<IEnumerable<DominioItem>> ObterSimNaoAsync()
            => _repository.ListarItensAsync("GEA_SIM_NÃO");

        public Task<IEnumerable<DominioItem>> ObterTiposAtividadeListadaAsync()
            => _repository.ListarItensAsync("EF_TX_TIPO_ATIVIDADE_LISTADA");

        public Task<IEnumerable<DominioItem>> ObterTiposDraListadoAsync()
            => _repository.ListarItensAsync("EF_TX_TIPO_DRA_LISTADO");

        public Task<IEnumerable<DominioItem>> ObterTiposAtividadeCptmAsync()
            => _repository.ListarItensAsync("EF_TX_TIPO_ATIVIDADE_CPTM");

        public Task<IEnumerable<DominioItem>> ObterLocaisAtividadeAsync()
            => _repository.ListarItensAsync("EF_TX_NM_LOCAL_ATIV");

        public Task<IEnumerable<DominioItem>> ObterOrigensEfluenteAsync()
            => _repository.ListarItensAsync("EF_TX_ORIGEM_EFLUENTE");

        public Task<IEnumerable<DominioItem>> ObterFontesGeradorasAsync()
            => _repository.ListarItensAsync("EF_TX_FONTE_GERADORA");

        public Task<IEnumerable<DominioItem>> ObterTiposDestinacaoAsync()
            => _repository.ListarItensAsync("EF_TX_TIPO_DESTINACAO");

        public Task<IEnumerable<DominioItem>> ObterTiposVeiculoAsync()
            => _repository.ListarItensAsync("EF_TX_TIPO_VEICULO");
    }
}