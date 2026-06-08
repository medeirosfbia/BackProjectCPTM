using ApiOracle.Models;
using ApiOracle.Repositories;

namespace ApiOracle.Services
{
    public class EfluenteService : IEfluenteService
    {
        private readonly IEfluenteRepository _repo;

        public EfluenteService(IEfluenteRepository repo)
        {
            _repo = repo;
        }

        public Task CriarTabelaAsync() => _repo.CriarTabelaAsync();

        public async Task<PtEfluente> CriarAsync(PtEfluenteCreateDto dto, int? usuarioId)
        {
            ValidateCoordinates(dto.NrLatGrauDecimalWgs84, dto.NrLongGrauDecimalWgs84);

            var efluente = Map(dto);
            efluente.PkCdMeioAmbienteCptm = string.IsNullOrWhiteSpace(dto.PkCdMeioAmbienteCptm)
                ? Guid.NewGuid().ToString("N")
                : dto.PkCdMeioAmbienteCptm.Trim();
            efluente.CreatedByUsuarioId = usuarioId;
            efluente.IsDeleted = 0;
            efluente.CreatedAt = DateTime.UtcNow;

            await _repo.InserirAsync(efluente);
            return efluente;
        }

        public async Task<bool> AtualizarAsync(string pk, PtEfluenteUpdateDto dto)
        {
            if (string.IsNullOrWhiteSpace(pk))
                throw new ArgumentException("PK obrigatorio");

            ValidateCoordinates(dto.NrLatGrauDecimalWgs84, dto.NrLongGrauDecimalWgs84);

            var existing = await _repo.GetByPkAsync(pk);
            if (existing == null) return false;

            var efluente = Map(dto);
            efluente.PkCdMeioAmbienteCptm = pk.Trim();
            efluente.CreatedAt = existing.CreatedAt;
            efluente.CreatedByUsuarioId = existing.CreatedByUsuarioId;

            return await _repo.AtualizarAsync(efluente);
        }

        public Task<bool> DeleteAsync(string pk, int? deletedByUsuarioId)
        {
            if (string.IsNullOrWhiteSpace(pk))
                throw new ArgumentException("PK obrigatorio");

            return _repo.DeleteAsync(pk.Trim(), deletedByUsuarioId);
        }

        public Task<bool> RestoreAsync(string pk)
        {
            if (string.IsNullOrWhiteSpace(pk))
                throw new ArgumentException("PK obrigatorio");

            return _repo.RestoreAsync(pk.Trim());
        }

        public Task<PtEfluente?> GetByPkAsync(string pk)
        {
            if (string.IsNullOrWhiteSpace(pk))
                throw new ArgumentException("PK obrigatorio");

            return _repo.GetByPkAsync(pk.Trim());
        }

        public Task<IEnumerable<PtEfluente>> ListarAsync(int page, int pageSize, string? municipio, string? linha, string? status, DateTime? data)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize switch
            {
                < 1 => 10,
                > 100 => 100,
                _ => pageSize
            };

            return _repo.ListarAsync(page, pageSize, municipio, linha, status, data);
        }

        public Task<IEnumerable<PtEfluente>> ListarPorUsuarioAsync(int usuarioId, int page, int pageSize, string? municipio, string? linha, string? status, DateTime? data)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize switch
            {
                < 1 => 10,
                > 100 => 100,
                _ => pageSize
            };

            return _repo.ListarPorUsuarioAsync(usuarioId, page, pageSize, municipio, linha, status, data);
        }

        public Task<IEnumerable<PtEfluente>> ListarAdminAsync(int page, int pageSize, string? municipio, string? linha, string? status, DateTime? data)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize switch
            {
                < 1 => 10,
                > 100 => 100,
                _ => pageSize
            };

            return _repo.ListarAdminAsync(page, pageSize, municipio, linha, status, data);
        }

        public Task<IEnumerable<PtEfluente>> ListarExcluidosAdminAsync(int page, int pageSize)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize switch
            {
                < 1 => 100,
                > 100 => 100,
                _ => pageSize
            };

            return _repo.ListarExcluidosAdminAsync(page, pageSize);
        }

        public Task<IEnumerable<PtEfluente>> ListarExcluidosPorUsuarioAsync(int usuarioId, int page, int pageSize)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize switch
            {
                < 1 => 100,
                > 100 => 100,
                _ => pageSize
            };

            return _repo.ListarExcluidosPorUsuarioAsync(usuarioId, page, pageSize);
        }

        public async Task<int> AnexarAsync(string pk, byte[] data, string? contentType, string? attName, long dataSize)
        {
            if (string.IsNullOrWhiteSpace(pk))
                throw new ArgumentException("PK obrigatorio");

            if (data.Length == 0 || dataSize <= 0)
                throw new ArgumentException("Arquivo vazio");

            var existing = await _repo.GetByPkAsync(pk);
            if (existing == null)
                throw new KeyNotFoundException("Registro nao encontrado");

            return await _repo.InserirAnexoAsync(pk.Trim(), data, contentType, attName, dataSize);
        }

        public Task<IEnumerable<RtEfluente>> ListarAnexosAsync(string pk)
        {
            if (string.IsNullOrWhiteSpace(pk))
                throw new ArgumentException("PK obrigatorio");

            return _repo.ListarAnexosAsync(pk.Trim());
        }

        public Task<RtEfluente?> ObterAnexoAsync(int attachmentId) => _repo.ObterAnexoAsync(attachmentId);

        private static void ValidateCoordinates(decimal? latitude, decimal? longitude)
        {
            if (latitude.HasValue && (latitude.Value < -90 || latitude.Value > 90))
                throw new ArgumentException("Latitude deve estar entre -90 e 90");

            if (longitude.HasValue && (longitude.Value < -180 || longitude.Value > 180))
                throw new ArgumentException("Longitude deve estar entre -180 e 180");
        }

        private static PtEfluente Map(PtEfluenteCreateDto dto) => new()
        {
            TxNrElementoMonitoramento = dto.TxNrElementoMonitoramento,
            TxNmElementoMonitoramento = dto.TxNmElementoMonitoramento,
            TxSiglaDeptoMeioAmbiente = dto.TxSiglaDeptoMeioAmbiente,
            TxStatusDoDesvioAmbiental = dto.TxStatusDoDesvioAmbiental,
            TxStatusDoRegistroNoBd = dto.TxStatusDoRegistroNoBd,
            TxMunicipio = dto.TxMunicipio,
            TxLinhaCptm = dto.TxLinhaCptm,
            TxViaCptm = dto.TxViaCptm,
            TxTrechoESentidoCptm = dto.TxTrechoESentidoCptm,
            TxKmPoste = dto.TxKmPoste,
            TxEstacaoCptm = dto.TxEstacaoCptm,
            NrLatGrauDecimalWgs84 = dto.NrLatGrauDecimalWgs84,
            NrLongGrauDecimalWgs84 = dto.NrLongGrauDecimalWgs84,
            NrLatMetrosSirgas2000 = dto.NrLatMetrosSirgas2000,
            NrLongMetrosSirgas2000 = dto.NrLongMetrosSirgas2000,
            TxNmLocalEscopoContratual = dto.TxNmLocalEscopoContratual,
            TxTipoDeFormulario = dto.TxTipoDeFormulario,
            DtDataEmissaoFormulario = dto.DtDataEmissaoFormulario,
            NrNumeroDeFormulario = dto.NrNumeroDeFormulario,
            TxAutorPfDoFormulario = dto.TxAutorPfDoFormulario,
            TxNaturezaDoPga = dto.TxNaturezaDoPga,
            TxNomePjExecutora = dto.TxNomePjExecutora,
            TxTipoAtividadeListada = dto.TxTipoAtividadeListada,
            TxTipoAtividadeNListada = dto.TxTipoAtividadeNListada,
            TxTipoDraListado = dto.TxTipoDraListado,
            TxTipoDraNListado = dto.TxTipoDraNListado,
            TxIdDra = dto.TxIdDra,
            DtValidadeDra = dto.DtValidadeDra,
            TxAnaliseCptmAprovacao = dto.TxAnaliseCptmAprovacao,
            TxTipoAtividadeCptm = dto.TxTipoAtividadeCptm,
            TxNmLocalAtiv = dto.TxNmLocalAtiv,
            TxNmLocalAtivComplemento = dto.TxNmLocalAtivComplemento,
            TxOrigemEfluente = dto.TxOrigemEfluente,
            TxFonteGeradora = dto.TxFonteGeradora,
            NrQuantidadeL = dto.NrQuantidadeL,
            TxTipoDestinacao = dto.TxTipoDestinacao,
            TxTipoVeiculo = dto.TxTipoVeiculo,
            TxIdVeiculo = dto.TxIdVeiculo,
            TxIdGuiaRemessa = dto.TxIdGuiaRemessa,
            NrDistanciaDaViaM = dto.NrDistanciaDaViaM,
            TxOfereceRiscoSistemaCptm = dto.TxOfereceRiscoSistemaCptm,
            TxProprietario = dto.TxProprietario,
            TxObsCadastramento = dto.TxObsCadastramento,
            DtDataDoCadastramento = dto.DtDataDoCadastramento,
            HrHoraDoCadastramento = dto.HrHoraDoCadastramento,
            TxAutorPjDoCadastro = dto.TxAutorPjDoCadastro,
            TxAutorPfDoCadastro = dto.TxAutorPfDoCadastro,
            TxNmResponsavelCadastro = dto.TxNmResponsavelCadastro,
            TxRpResponsavelCadastro = dto.TxRpResponsavelCadastro,
            TxDrtResponsavelCadastro = dto.TxDrtResponsavelCadastro,
            TxNomePjDaContratada = dto.TxNomePjDaContratada,
            TxNrContratoContratada = dto.TxNrContratoContratada,
            TxNmAreaGestoraCptm = dto.TxNmAreaGestoraCptm,
            TxIdAreaGestoraCptm = dto.TxIdAreaGestoraCptm,
            TxSiglaAreaGestoraCptm = dto.TxSiglaAreaGestoraCptm,
            TxNomePfDaRepresentante = dto.TxNomePfDaRepresentante,
            TxNomePjDaSupervisora = dto.TxNomePjDaSupervisora,
            TxNrContratoSupervisora = dto.TxNrContratoSupervisora,
            TxNmArquivoFdcRelacionado = dto.TxNmArquivoFdcRelacionado,
            PkCdArquivoFdcRelacionado = dto.PkCdArquivoFdcRelacionado,
            TxNmArquivoRvtRelacionado = dto.TxNmArquivoRvtRelacionado,
            PkCdElementoDeMonitorRvt = dto.PkCdElementoDeMonitorRvt,
            TxNmArquivoDacRelacionado = dto.TxNmArquivoDacRelacionado,
            PkCdElementoDeMonitorDac = dto.PkCdElementoDeMonitorDac,
            TxNmArquivoCncRelacionado = dto.TxNmArquivoCncRelacionado,
            PkCdElementoDeMonitorCnc = dto.PkCdElementoDeMonitorCnc,
            PkCdCodigoNoUltimoRra = dto.PkCdCodigoNoUltimoRra,
            PkCdCedoc = dto.PkCdCedoc,
            TxNomeFoto01 = dto.TxNomeFoto01,
            TxNomeFoto02 = dto.TxNomeFoto02,
            TxNomeFoto03 = dto.TxNomeFoto03,
            TxNomeFoto04 = dto.TxNomeFoto04
        };
    }
}
