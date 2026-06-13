using System.Data;
using ApiOracle.Data;
using ApiOracle.Models;
using Dapper;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;

namespace ApiOracle.Repositories
{
    public class EfluenteRepository : IEfluenteRepository
    {
        private readonly OracleConnectionFactory _factory;
        private readonly ILogger<EfluenteRepository> _logger;

        private static readonly (string Column, string Property)[] EfluenteColumns =
        {
            ("PK_CD_MEIO_AMBIENTE_CPTM", "PkCdMeioAmbienteCptm"),
            ("TX_NR_ELEMENTO_MONITORAMENTO", "TxNrElementoMonitoramento"),
            ("TX_NM_ELEMENTO_MONITORAMENTO", "TxNmElementoMonitoramento"),
            ("TX_SIGLA_DEPTO_MEIO_AMBIENTE", "TxSiglaDeptoMeioAmbiente"),
            ("TX_STATUS_DO_DESVIO_AMBIENTAL", "TxStatusDoDesvioAmbiental"),
            ("TX_STATUS_DO_REGISTRO_NO_BD", "TxStatusDoRegistroNoBd"),
            ("TX_MUNICIPIO", "TxMunicipio"),
            ("TX_LINHA_CPTM", "TxLinhaCptm"),
            ("TX_VIA_CPTM", "TxViaCptm"),
            ("TX_TRECHO_E_SENTIDO_CPTM", "TxTrechoESentidoCptm"),
            ("TX_KM_POSTE", "TxKmPoste"),
            ("TX_ESTACAO_CPTM", "TxEstacaoCptm"),
            ("NR_LAT_GRAU_DECIMAL_WGS84", "NrLatGrauDecimalWgs84"),
            ("NR_LONG_GRAU_DECIMAL_WGS84", "NrLongGrauDecimalWgs84"),
            ("NR_LAT_METROS_SIRGAS2000", "NrLatMetrosSirgas2000"),
            ("NR_LONG_METROS_SIRGAS2000", "NrLongMetrosSirgas2000"),
            ("TX_NM_LOCAL_ESCOPO_CONTRATUAL", "TxNmLocalEscopoContratual"),
            ("TX_TIPO_DE_FORMULARIO", "TxTipoDeFormulario"),
            ("DT_DATA_EMISSAO_FORMULARIO", "DtDataEmissaoFormulario"),
            ("NR_NUMERO_DE_FORMULARIO", "NrNumeroDeFormulario"),
            ("TX_AUTOR_PF_DO_FORMULARIO", "TxAutorPfDoFormulario"),
            ("TX_NATUREZA_DO_PGA", "TxNaturezaDoPga"),
            ("TX_NOME_PJ_EXECUTORA", "TxNomePjExecutora"),
            ("TX_TIPO_ATIVIDADE_LISTADA", "TxTipoAtividadeListada"),
            ("TX_TIPO_ATIVIDADE_N_LISTADA", "TxTipoAtividadeNListada"),
            ("TX_TIPO_DRA_LISTADO", "TxTipoDraListado"),
            ("TX_TIPO_DRA_N_LISTADO", "TxTipoDraNListado"),
            ("TX_ID_DRA", "TxIdDra"),
            ("DT_VALIDADE_DRA", "DtValidadeDra"),
            ("TX_ANALISE_CPTM_APROVACAO", "TxAnaliseCptmAprovacao"),
            ("TX_TIPO_ATIVIDADE_CPTM", "TxTipoAtividadeCptm"),
            ("TX_NM_LOCAL_ATIV", "TxNmLocalAtiv"),
            ("TX_NM_LOCAL_ATIV_COMPLEMENTO", "TxNmLocalAtivComplemento"),
            ("TX_ORIGEM_EFLUENTE", "TxOrigemEfluente"),
            ("TX_FONTE_GERADORA", "TxFonteGeradora"),
            ("NR_QUANTIDADE_L", "NrQuantidadeL"),
            ("TX_TIPO_DESTINACAO", "TxTipoDestinacao"),
            ("TX_TIPO_VEICULO", "TxTipoVeiculo"),
            ("TX_ID_VEICULO", "TxIdVeiculo"),
            ("TX_ID_GUIA_REMESSA", "TxIdGuiaRemessa"),
            ("NR_DISTANCIA_DA_VIA_M", "NrDistanciaDaViaM"),
            ("TX_OFERECE_RISCO_SISTEMA_CPTM", "TxOfereceRiscoSistemaCptm"),
            ("TX_PROPRIETARIO", "TxProprietario"),
            ("TX_OBS_CADASTRAMENTO", "TxObsCadastramento"),
            ("DT_DATA_DO_CADASTRAMENTO", "DtDataDoCadastramento"),
            ("HR_HORA_DO_CADASTRAMENTO", "HrHoraDoCadastramento"),
            ("TX_AUTOR_PJ_DO_CADASTRO", "TxAutorPjDoCadastro"),
            ("TX_AUTOR_PF_DO_CADASTRO", "TxAutorPfDoCadastro"),
            ("TX_NM_RESPONSAVEL_CADASTRO", "TxNmResponsavelCadastro"),
            ("TX_RP_RESPONSAVEL_CADASTRO", "TxRpResponsavelCadastro"),
            ("TX_DRT_RESPONSAVEL_CADASTRO", "TxDrtResponsavelCadastro"),
            ("TX_NOME_PJ_DA_CONTRATADA", "TxNomePjDaContratada"),
            ("TX_NR_CONTRATO_CONTRATADA", "TxNrContratoContratada"),
            ("TX_NM_AREA_GESTORA_CPTM", "TxNmAreaGestoraCptm"),
            ("TX_ID_AREA_GESTORA_CPTM", "TxIdAreaGestoraCptm"),
            ("TX_SIGLA_AREA_GESTORA_CPTM", "TxSiglaAreaGestoraCptm"),
            ("TX_NOME_PF_DA_REPRESENTANTE", "TxNomePfDaRepresentante"),
            ("TX_NOME_PJ_DA_SUPERVISORA", "TxNomePjDaSupervisora"),
            ("TX_NR_CONTRATO_SUPERVISORA", "TxNrContratoSupervisora"),
            ("TX_NM_ARQUIVO_FDC_RELACIONADO", "TxNmArquivoFdcRelacionado"),
            ("PK_CD_ARQUIVO_FDC_RELACIONADO", "PkCdArquivoFdcRelacionado"),
            ("TX_NM_ARQUIVO_RVT_RELACIONADO", "TxNmArquivoRvtRelacionado"),
            ("PK_CD_ELEMENTO_DE_MONITOR_RVT", "PkCdElementoDeMonitorRvt"),
            ("TX_NM_ARQUIVO_DAC_RELACIONADO", "TxNmArquivoDacRelacionado"),
            ("PK_CD_ELEMENTO_DE_MONITOR_DAC", "PkCdElementoDeMonitorDac"),
            ("TX_NM_ARQUIVO_CNC_RELACIONADO", "TxNmArquivoCncRelacionado"),
            ("PK_CD_ELEMENTO_DE_MONITOR_CNC", "PkCdElementoDeMonitorCnc"),
            ("PK_CD_CODIGO_NO_ULTIMO_RRA", "PkCdCodigoNoUltimoRra"),
            ("PK_CD_CEDOC", "PkCdCedoc"),
            ("TX_NOME_FOTO_01", "TxNomeFoto01"),
            ("TX_NOME_FOTO_02", "TxNomeFoto02"),
            ("TX_NOME_FOTO_03", "TxNomeFoto03"),
            ("TX_NOME_FOTO_04", "TxNomeFoto04"),
            ("CREATED_BY_USUARIO_ID", "CreatedByUsuarioId"),
            ("IS_DELETED", "IsDeleted"),
            ("DELETED_AT", "DeletedAt"),
            ("DELETED_BY", "DeletedBy"),
            ("CREATED_AT", "CreatedAt"),
            ("UPDATED_AT", "UpdatedAt")
        };
        private static readonly HashSet<string> UpdateIgnoredColumns = new(StringComparer.OrdinalIgnoreCase)
        {
            "PK_CD_MEIO_AMBIENTE_CPTM",
            "CREATED_AT",
            "CREATED_BY_USUARIO_ID",
            "UPDATED_AT",
            "IS_DELETED",
            "DELETED_AT",
            "DELETED_BY"
        };

        public EfluenteRepository(OracleConnectionFactory factory, ILogger<EfluenteRepository> logger)
        {
            _factory = factory;
            _logger = logger;
        }

        public async Task CriarTabelaAsync()
        {
            using var conn = _factory.CreateConnection();
            await conn.ExecuteAsync(EfluenteDatabaseSql.CreatePtEfluente);
            await conn.ExecuteAsync(EfluenteDatabaseSql.CreateRtEfluente);
            await conn.ExecuteAsync(EfluenteDatabaseSql.CreateSequence);
            await conn.ExecuteAsync(EfluenteDatabaseSql.AddIsDeletedColumn);
            await conn.ExecuteAsync(EfluenteDatabaseSql.AddDeletedAtColumn);
            await conn.ExecuteAsync(EfluenteDatabaseSql.AddDeletedByColumn);
            await conn.ExecuteAsync(EfluenteDatabaseSql.NormalizeIsDeletedColumn);
            await conn.ExecuteAsync(EfluenteDatabaseSql.DropSpatialIndexIfExists);
        }

        public async Task<string> InserirAsync(PtEfluente efluente)
        {
            using var conn = _factory.CreateConnection();
            var insertColumns = EfluenteColumns.Where(c => c.Column != "UPDATED_AT").Select(c => c.Column).Concat(new[] { "GEOMETRY" });
            var values = EfluenteColumns
                .Where(c => c.Column != "UPDATED_AT")
                .Select(c => $":{c.Property}")
                .Concat(new[] { GeometrySql });

            var sql = $@"
                INSERT INTO PT_EFLUENTE ({string.Join(", ", insertColumns)})
                VALUES ({string.Join(", ", values)})";

            try
            {
                await conn.ExecuteAsync(sql, efluente);
            }
            catch (OracleException ex) when (ex.Number == 29861)
            {
                await conn.ExecuteAsync(EfluenteDatabaseSql.DropSpatialIndexIfExists);
                await conn.ExecuteAsync(sql, efluente);
            }

            return efluente.PkCdMeioAmbienteCptm;
        }

        public async Task<bool> AtualizarAsync(PtEfluente efluente)
        {
            using var conn = _factory.CreateConnection();
            var setters = EfluenteColumns
                .Where(c => !UpdateIgnoredColumns.Contains(c.Column))
                .Select(c => $"{c.Column} = :{c.Property}")
                .Concat(new[] { $"GEOMETRY = {GeometrySql}", "UPDATED_AT = SYSDATE" });

            var sql = $@"
                UPDATE PT_EFLUENTE
                SET {string.Join(", ", setters)}
                WHERE PK_CD_MEIO_AMBIENTE_CPTM = :PkCdMeioAmbienteCptm
                  AND (IS_DELETED = 0 OR IS_DELETED IS NULL)";

            try
            {
                return await conn.ExecuteAsync(sql, efluente) > 0;
            }
            catch (OracleException ex) when (ex.Number == 29861)
            {
                await conn.ExecuteAsync(EfluenteDatabaseSql.DropSpatialIndexIfExists);
                return await conn.ExecuteAsync(sql, efluente) > 0;
            }
        }

        public async Task<bool> DeleteAsync(string pk, int? deletedByUsuarioId)
        {
            using var conn = _factory.CreateConnection();
            return await conn.ExecuteAsync(
                @"UPDATE PT_EFLUENTE
                  SET IS_DELETED = 1,
                      DELETED_AT = SYSDATE,
                      DELETED_BY = :DeletedByUsuarioId,
                      UPDATED_AT = SYSDATE
                  WHERE PK_CD_MEIO_AMBIENTE_CPTM = :Pk
                    AND (IS_DELETED = 0 OR IS_DELETED IS NULL)",
                new { Pk = pk, DeletedByUsuarioId = deletedByUsuarioId }) > 0;
        }

        public async Task<bool> RestoreAsync(string pk)
        {
            using var conn = _factory.CreateConnection();
            return await conn.ExecuteAsync(
                @"UPDATE PT_EFLUENTE
                  SET IS_DELETED = 0,
                      DELETED_AT = NULL,
                      DELETED_BY = NULL,
                      UPDATED_AT = SYSDATE
                  WHERE PK_CD_MEIO_AMBIENTE_CPTM = :Pk
                    AND IS_DELETED = 1",
                new { Pk = pk }) > 0;
        }

        public async Task<PtEfluente?> GetByPkAsync(string pk)
        {
            using var conn = _factory.CreateConnection();
            var sql = $@"
                SELECT {SelectColumns}
                FROM PT_EFLUENTE
                WHERE PK_CD_MEIO_AMBIENTE_CPTM = :Pk
                  AND (IS_DELETED = 0 OR IS_DELETED IS NULL)";

            return await conn.QueryFirstOrDefaultAsync<PtEfluente>(sql, new { Pk = pk });
        }

        public async Task<IEnumerable<PtEfluente>> ListarAsync(int page, int pageSize, string? municipio, string? linha, string? status, DateTime? data)
        {
            using var conn = _factory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("Offset", (page - 1) * pageSize);
            parameters.Add("PageSize", pageSize);

            var where = new List<string>();
            where.Add("(IS_DELETED = 0 OR IS_DELETED IS NULL)");
            if (!string.IsNullOrWhiteSpace(municipio))
            {
                where.Add("UPPER(TX_MUNICIPIO) LIKE UPPER(:Municipio)");
                parameters.Add("Municipio", $"%{municipio.Trim()}%");
            }
            if (!string.IsNullOrWhiteSpace(linha))
            {
                where.Add("UPPER(TX_LINHA_CPTM) LIKE UPPER(:Linha)");
                parameters.Add("Linha", $"%{linha.Trim()}%");
            }
            if (!string.IsNullOrWhiteSpace(status))
            {
                where.Add("(UPPER(TX_STATUS_DO_DESVIO_AMBIENTAL) LIKE UPPER(:Status) OR UPPER(TX_STATUS_DO_REGISTRO_NO_BD) LIKE UPPER(:Status))");
                parameters.Add("Status", $"%{status.Trim()}%");
            }
            if (data.HasValue)
            {
                where.Add("TRUNC(DT_DATA_DO_CADASTRAMENTO) = TRUNC(:DataFiltro)");
                parameters.Add("DataFiltro", data.Value.Date);
            }

            var sql = $@"
                SELECT {SelectColumns}
                FROM PT_EFLUENTE
                {(where.Count > 0 ? "WHERE " + string.Join(" AND ", where) : string.Empty)}
                ORDER BY CREATED_AT DESC
                OFFSET :Offset ROWS FETCH NEXT :PageSize ROWS ONLY";

            _logger.LogInformation(
                "Query efluentes executada usuarioFiltro={UsuarioFiltro} page={Page} pageSize={PageSize} municipio={Municipio} linha={Linha} status={Status} data={Data} sql={Sql}",
                null,
                page,
                pageSize,
                municipio,
                linha,
                status,
                data?.ToString("yyyy-MM-dd"),
                sql);

            var result = (await conn.QueryAsync<PtEfluente>(sql, parameters)).ToList();
            _logger.LogInformation("Query efluentes retornou quantidade={Quantidade} usuarioFiltro={UsuarioFiltro}", result.Count, null);
            return result;
        }

        public Task<IEnumerable<PtEfluente>> ListarPorUsuarioAsync(int usuarioId, int page, int pageSize, string? municipio, string? linha, string? status, DateTime? data)
        {
            return ListarFiltradoAsync(usuarioId, page, pageSize, municipio, linha, status, data);
        }

        public async Task<PtEfluente?> ObterUltimaInspecaoPorUsuarioAsync(int usuarioId)
        {
            using var conn = _factory.CreateConnection();
            var sql = $@"
                SELECT {SelectColumns}
                FROM PT_EFLUENTE
                WHERE CREATED_BY_USUARIO_ID = :UsuarioId
                  AND (IS_DELETED = 0 OR IS_DELETED IS NULL)
                ORDER BY
                    DT_DATA_DO_CADASTRAMENTO DESC NULLS LAST,
                    HR_HORA_DO_CADASTRAMENTO DESC NULLS LAST,
                    CREATED_AT DESC,
                    PK_CD_MEIO_AMBIENTE_CPTM DESC
                FETCH FIRST 1 ROWS ONLY";

            _logger.LogInformation(
                "Query ultima inspecao efluente executada usuarioFiltro={UsuarioFiltro} sql={Sql}",
                usuarioId,
                sql);

            var result = await conn.QueryFirstOrDefaultAsync<PtEfluente>(sql, new { UsuarioId = usuarioId });
            _logger.LogInformation(
                "Query ultima inspecao efluente retornou encontrado={Encontrado} usuarioFiltro={UsuarioFiltro}",
                result != null,
                usuarioId);

            return result;
        }

        public Task<IEnumerable<PtEfluente>> ListarAdminAsync(int page, int pageSize, string? municipio, string? linha, string? status, DateTime? data)
        {
            return ListarFiltradoAsync(null, page, pageSize, municipio, linha, status, data);
        }

        public Task<IEnumerable<PtEfluente>> ListarExcluidosAdminAsync(int page, int pageSize)
        {
            return ListarExcluidosAsync(null, page, pageSize);
        }

        public Task<IEnumerable<PtEfluente>> ListarExcluidosPorUsuarioAsync(int usuarioId, int page, int pageSize)
        {
            return ListarExcluidosAsync(usuarioId, page, pageSize);
        }

        private async Task<IEnumerable<PtEfluente>> ListarFiltradoAsync(int? usuarioId, int page, int pageSize, string? municipio, string? linha, string? status, DateTime? data)
        {
            using var conn = _factory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("Offset", (page - 1) * pageSize);
            parameters.Add("PageSize", pageSize);

            var where = new List<string> { "(IS_DELETED = 0 OR IS_DELETED IS NULL)" };

            if (usuarioId.HasValue)
            {
                where.Add("CREATED_BY_USUARIO_ID = :UsuarioId");
                parameters.Add("UsuarioId", usuarioId.Value);
            }

            if (!string.IsNullOrWhiteSpace(municipio))
            {
                where.Add("UPPER(TX_MUNICIPIO) LIKE UPPER(:Municipio)");
                parameters.Add("Municipio", $"%{municipio.Trim()}%");
            }
            if (!string.IsNullOrWhiteSpace(linha))
            {
                where.Add("UPPER(TX_LINHA_CPTM) LIKE UPPER(:Linha)");
                parameters.Add("Linha", $"%{linha.Trim()}%");
            }
            if (!string.IsNullOrWhiteSpace(status))
            {
                where.Add("(UPPER(TX_STATUS_DO_DESVIO_AMBIENTAL) LIKE UPPER(:Status) OR UPPER(TX_STATUS_DO_REGISTRO_NO_BD) LIKE UPPER(:Status))");
                parameters.Add("Status", $"%{status.Trim()}%");
            }
            if (data.HasValue)
            {
                where.Add("TRUNC(DT_DATA_DO_CADASTRAMENTO) = TRUNC(:DataFiltro)");
                parameters.Add("DataFiltro", data.Value.Date);
            }

            var sql = $@"
                SELECT {SelectColumns}
                FROM PT_EFLUENTE
                WHERE {string.Join(" AND ", where)}
                ORDER BY CREATED_AT DESC
                OFFSET :Offset ROWS FETCH NEXT :PageSize ROWS ONLY";

            _logger.LogInformation(
                "Query efluentes executada usuarioFiltro={UsuarioFiltro} page={Page} pageSize={PageSize} municipio={Municipio} linha={Linha} status={Status} data={Data} sql={Sql}",
                usuarioId,
                page,
                pageSize,
                municipio,
                linha,
                status,
                data?.ToString("yyyy-MM-dd"),
                sql);

            var result = (await conn.QueryAsync<PtEfluente>(sql, parameters)).ToList();
            _logger.LogInformation("Query efluentes retornou quantidade={Quantidade} usuarioFiltro={UsuarioFiltro}", result.Count, usuarioId);
            return result;
        }

        private async Task<IEnumerable<PtEfluente>> ListarExcluidosAsync(int? usuarioId, int page, int pageSize)
        {
            using var conn = _factory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("Offset", (page - 1) * pageSize);
            parameters.Add("PageSize", pageSize);

            var where = new List<string> { "IS_DELETED = 1" };
            if (usuarioId.HasValue)
            {
                where.Add("CREATED_BY_USUARIO_ID = :UsuarioId");
                parameters.Add("UsuarioId", usuarioId.Value);
            }

            var sql = $@"
                SELECT {SelectColumns}
                FROM PT_EFLUENTE
                WHERE {string.Join(" AND ", where)}
                ORDER BY DELETED_AT DESC
                OFFSET :Offset ROWS FETCH NEXT :PageSize ROWS ONLY";

            _logger.LogInformation(
                "Query efluentes excluidos executada usuarioFiltro={UsuarioFiltro} page={Page} pageSize={PageSize} sql={Sql}",
                usuarioId,
                page,
                pageSize,
                sql);

            var result = (await conn.QueryAsync<PtEfluente>(sql, parameters)).ToList();
            _logger.LogInformation("Query efluentes excluidos retornou quantidade={Quantidade} usuarioFiltro={UsuarioFiltro}", result.Count, usuarioId);
            return result;
        }

        public async Task<int> InserirAnexoAsync(string pk, byte[] data, string? contentType, string? attName, long dataSize)
        {
            using var conn = _factory.CreateConnection();
            var sql = @"
                INSERT INTO RT_EFLUENTE (ATTACHMENTID, REL_OBJECTID, CONTENT_TYPE, ATT_NAME, DATA_SIZE, DATA, CREATED_AT)
                VALUES (SQ_RT_EFLUENTE.NEXTVAL, :RelObjectId, :ContentType, :AttName, :DataSize, :Data, SYSDATE)
                RETURNING ATTACHMENTID INTO :AttachmentId";

            var parameters = new DynamicParameters();
            parameters.Add("RelObjectId", pk);
            parameters.Add("ContentType", contentType);
            parameters.Add("AttName", attName);
            parameters.Add("DataSize", dataSize);
            parameters.Add("Data", data);
            parameters.Add("AttachmentId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await conn.ExecuteAsync(sql, parameters);
            return parameters.Get<int>("AttachmentId");
        }

        public async Task<IEnumerable<RtEfluente>> ListarAnexosAsync(string pk)
        {
            using var conn = _factory.CreateConnection();
            var sql = @"
                SELECT
                    RT.ATTACHMENTID AS AttachmentId,
                    RT.REL_OBJECTID AS RelObjectId,
                    RT.CONTENT_TYPE AS ContentType,
                    RT.ATT_NAME AS AttName,
                    RT.DATA_SIZE AS DataSize,
                    RT.CREATED_AT AS CreatedAt
                FROM RT_EFLUENTE RT
                INNER JOIN PT_EFLUENTE PT ON PT.PK_CD_MEIO_AMBIENTE_CPTM = RT.REL_OBJECTID
                WHERE RT.REL_OBJECTID = :Pk
                  AND (PT.IS_DELETED = 0 OR PT.IS_DELETED IS NULL)
                ORDER BY RT.CREATED_AT ASC, RT.ATTACHMENTID ASC";

            return await conn.QueryAsync<RtEfluente>(sql, new { Pk = pk });
        }

        public async Task<RtEfluente?> ObterAnexoAsync(int attachmentId)
        {
            using var conn = _factory.CreateConnection();
            var sql = @"
                SELECT
                    RT.ATTACHMENTID AS AttachmentId,
                    RT.REL_OBJECTID AS RelObjectId,
                    RT.CONTENT_TYPE AS ContentType,
                    RT.ATT_NAME AS AttName,
                    RT.DATA_SIZE AS DataSize,
                    RT.DATA AS Data,
                    RT.CREATED_AT AS CreatedAt
                FROM RT_EFLUENTE RT
                INNER JOIN PT_EFLUENTE PT ON PT.PK_CD_MEIO_AMBIENTE_CPTM = RT.REL_OBJECTID
                WHERE RT.ATTACHMENTID = :AttachmentId
                  AND (PT.IS_DELETED = 0 OR PT.IS_DELETED IS NULL)";

            return await conn.QueryFirstOrDefaultAsync<RtEfluente>(sql, new { AttachmentId = attachmentId });
        }

        private static string SelectColumns =>
            string.Join(", ", EfluenteColumns.Select(c => $"{c.Column} AS {c.Property}"))
            + ", (SELECT COUNT(1) FROM RT_EFLUENTE RT WHERE RT.REL_OBJECTID = PT_EFLUENTE.PK_CD_MEIO_AMBIENTE_CPTM) AS AttachmentCount";

        private const string GeometrySql = @"
            CASE
                WHEN :NrLatGrauDecimalWgs84 IS NOT NULL AND :NrLongGrauDecimalWgs84 IS NOT NULL THEN
                    MDSYS.SDO_GEOMETRY(
                        2001,
                        4326,
                        MDSYS.SDO_POINT_TYPE(:NrLongGrauDecimalWgs84, :NrLatGrauDecimalWgs84, NULL),
                        NULL,
                        NULL
                    )
                ELSE NULL
            END";
    }

    internal static class EfluenteDatabaseSql
    {
        public const string CreatePtEfluente = @"
BEGIN
    EXECUTE IMMEDIATE '
        CREATE TABLE PT_EFLUENTE (
            PK_CD_MEIO_AMBIENTE_CPTM VARCHAR2(255) NOT NULL PRIMARY KEY,
            TX_NR_ELEMENTO_MONITORAMENTO VARCHAR2(255),
            TX_NM_ELEMENTO_MONITORAMENTO VARCHAR2(255),
            TX_SIGLA_DEPTO_MEIO_AMBIENTE NUMBER,
            TX_STATUS_DO_DESVIO_AMBIENTAL NUMBER,
            TX_STATUS_DO_REGISTRO_NO_BD NUMBER,
            TX_MUNICIPIO NUMBER,
            TX_LINHA_CPTM NUMBER,
            TX_VIA_CPTM NUMBER,
            TX_TRECHO_E_SENTIDO_CPTM NUMBER,
            TX_KM_POSTE VARCHAR2(255),
            TX_ESTACAO_CPTM NUMBER,
            NR_LAT_GRAU_DECIMAL_WGS84 NUMBER(18,8),
            NR_LONG_GRAU_DECIMAL_WGS84 NUMBER(18,8),
            NR_LAT_METROS_SIRGAS2000 NUMBER(18,8),
            NR_LONG_METROS_SIRGAS2000 NUMBER(18,8),
            TX_NM_LOCAL_ESCOPO_CONTRATUAL VARCHAR2(255),
            TX_TIPO_DE_FORMULARIO VARCHAR2(255),
            DT_DATA_EMISSAO_FORMULARIO DATE,
            NR_NUMERO_DE_FORMULARIO NUMBER,
            TX_AUTOR_PF_DO_FORMULARIO VARCHAR2(255),
            TX_NATUREZA_DO_PGA NUMBER,
            TX_NOME_PJ_EXECUTORA VARCHAR2(255),
            TX_TIPO_ATIVIDADE_LISTADA NUMBER,
            TX_TIPO_ATIVIDADE_N_LISTADA VARCHAR2(255),
            TX_TIPO_DRA_LISTADO NUMBER,
            TX_TIPO_DRA_N_LISTADO VARCHAR2(255),
            TX_ID_DRA VARCHAR2(255),
            DT_VALIDADE_DRA DATE,
            TX_ANALISE_CPTM_APROVACAO VARCHAR2(255),
            TX_TIPO_ATIVIDADE_CPTM NUMBER,
            TX_NM_LOCAL_ATIV NUMBER,
            TX_NM_LOCAL_ATIV_COMPLEMENTO VARCHAR2(255),
            TX_ORIGEM_EFLUENTE NUMBER,
            TX_FONTE_GERADORA NUMBER,
            NR_QUANTIDADE_L NUMBER,
            TX_TIPO_DESTINACAO NUMBER,
            TX_TIPO_VEICULO NUMBER,
            TX_ID_VEICULO VARCHAR2(255),
            TX_ID_GUIA_REMESSA VARCHAR2(255),
            NR_DISTANCIA_DA_VIA_M NUMBER(18,8),
            TX_OFERECE_RISCO_SISTEMA_CPTM VARCHAR2(255),
            TX_PROPRIETARIO NUMBER,
            TX_OBS_CADASTRAMENTO VARCHAR2(2000),
            DT_DATA_DO_CADASTRAMENTO DATE,
            HR_HORA_DO_CADASTRAMENTO VARCHAR2(5),
            TX_AUTOR_PJ_DO_CADASTRO VARCHAR2(255),
            TX_AUTOR_PF_DO_CADASTRO VARCHAR2(255),
            TX_NM_RESPONSAVEL_CADASTRO VARCHAR2(255),
            TX_RP_RESPONSAVEL_CADASTRO VARCHAR2(255),
            TX_DRT_RESPONSAVEL_CADASTRO VARCHAR2(255),
            TX_NOME_PJ_DA_CONTRATADA VARCHAR2(255),
            TX_NR_CONTRATO_CONTRATADA VARCHAR2(255),
            TX_NM_AREA_GESTORA_CPTM NUMBER,
            TX_ID_AREA_GESTORA_CPTM VARCHAR2(255),
            TX_SIGLA_AREA_GESTORA_CPTM VARCHAR2(255),
            TX_NOME_PF_DA_REPRESENTANTE VARCHAR2(255),
            TX_NOME_PJ_DA_SUPERVISORA VARCHAR2(255),
            TX_NR_CONTRATO_SUPERVISORA VARCHAR2(255),
            TX_NM_ARQUIVO_FDC_RELACIONADO VARCHAR2(255),
            PK_CD_ARQUIVO_FDC_RELACIONADO VARCHAR2(255),
            TX_NM_ARQUIVO_RVT_RELACIONADO VARCHAR2(255),
            PK_CD_ELEMENTO_DE_MONITOR_RVT VARCHAR2(255),
            TX_NM_ARQUIVO_DAC_RELACIONADO VARCHAR2(255),
            PK_CD_ELEMENTO_DE_MONITOR_DAC VARCHAR2(255),
            TX_NM_ARQUIVO_CNC_RELACIONADO VARCHAR2(255),
            PK_CD_ELEMENTO_DE_MONITOR_CNC VARCHAR2(255),
            PK_CD_CODIGO_NO_ULTIMO_RRA VARCHAR2(255),
            PK_CD_CEDOC VARCHAR2(255),
            TX_NOME_FOTO_01 VARCHAR2(255),
            TX_NOME_FOTO_02 VARCHAR2(255),
            TX_NOME_FOTO_03 VARCHAR2(255),
            TX_NOME_FOTO_04 VARCHAR2(255),
            GEOMETRY SDO_GEOMETRY,
            CREATED_BY_USUARIO_ID NUMBER,
            IS_DELETED NUMBER(1) DEFAULT 0,
            DELETED_AT DATE,
            DELETED_BY NUMBER,
            CREATED_AT DATE DEFAULT SYSDATE NOT NULL,
            UPDATED_AT DATE,
            -- Foreign Keys para Tabelas de Domínio
            CONSTRAINT FK_EFL_SIGLA_DEPTO FOREIGN KEY (TX_SIGLA_DEPTO_MEIO_AMBIENTE) REFERENCES GEA_TX_SIGLA_DEPTO_MEIO_AMBIENTE(CODIGO),
            CONSTRAINT FK_EFL_STATUS_DESVIO FOREIGN KEY (TX_STATUS_DO_DESVIO_AMBIENTAL) REFERENCES GEA_TX_STATUS_DO_DESVIO_AMBIENTAL(CODIGO),
            CONSTRAINT FK_EFL_STATUS_BD FOREIGN KEY (TX_STATUS_DO_REGISTRO_NO_BD) REFERENCES GEA_TX_STATUS_DO_REGISTRO_NO_BD(CODIGO),
            CONSTRAINT FK_EFL_MUNICIPIO FOREIGN KEY (TX_MUNICIPIO) REFERENCES GEA_TX_MUNICIPIO(CODIGO),
            CONSTRAINT FK_EFL_LINHA FOREIGN KEY (TX_LINHA_CPTM) REFERENCES GEA_TX_LINHA_CPTM(CODIGO),
            CONSTRAINT FK_EFL_VIA FOREIGN KEY (TX_VIA_CPTM) REFERENCES GEA_TX_VIA_CPTM(CODIGO),
            CONSTRAINT FK_EFL_TRECHO FOREIGN KEY (TX_TRECHO_E_SENTIDO_CPTM) REFERENCES GEA_TX_TRECHO_E_SENTIDO_CPTM(CODIGO),
            CONSTRAINT FK_EFL_ESTACAO FOREIGN KEY (TX_ESTACAO_CPTM) REFERENCES GEA_TX_ESTACAO_CPTM(CODIGO),
            CONSTRAINT FK_EFL_NATUREZA FOREIGN KEY (TX_NATUREZA_DO_PGA) REFERENCES GEA_TX_NATUREZA_DO_PGA(CODIGO),
            CONSTRAINT FK_EFL_ATIV_LIST FOREIGN KEY (TX_TIPO_ATIVIDADE_LISTADA) REFERENCES EF_TX_TIPO_ATIVIDADE_LISTADA(CODIGO),
            CONSTRAINT FK_EFL_DRA_LIST FOREIGN KEY (TX_TIPO_DRA_LISTADO) REFERENCES EF_TX_TIPO_DRA_LISTADO(CODIGO),
            CONSTRAINT FK_EFL_ATIV_CPTM FOREIGN KEY (TX_TIPO_ATIVIDADE_CPTM) REFERENCES EF_TX_TIPO_ATIVIDADE_CPTM(CODIGO),
            CONSTRAINT FK_EFL_LOCAL_ATIV FOREIGN KEY (TX_NM_LOCAL_ATIV) REFERENCES EF_TX_NM_LOCAL_ATIV(CODIGO),
            CONSTRAINT FK_EFL_ORIGEM FOREIGN KEY (TX_ORIGEM_EFLUENTE) REFERENCES EF_TX_ORIGEM_EFLUENTE(CODIGO),
            CONSTRAINT FK_EFL_FONTE FOREIGN KEY (TX_FONTE_GERADORA) REFERENCES EF_TX_FONTE_GERADORA(CODIGO),
            CONSTRAINT FK_EFL_DESTINACAO FOREIGN KEY (TX_TIPO_DESTINACAO) REFERENCES EF_TX_TIPO_DESTINACAO(CODIGO),
            CONSTRAINT FK_EFL_VEICULO FOREIGN KEY (TX_TIPO_VEICULO) REFERENCES EF_TX_TIPO_VEICULO(CODIGO),
            CONSTRAINT FK_EFL_PROPRIETARIO FOREIGN KEY (TX_PROPRIETARIO) REFERENCES GEA_TX_PROPRIETARIO(CODIGO),
            CONSTRAINT FK_EFL_AREA_GESTORA FOREIGN KEY (TX_NM_AREA_GESTORA_CPTM) REFERENCES GEA_TX_NM_AREA_GESTORA_CPTM(CODIGO)
        )';
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE != -955 THEN
            RAISE;
        END IF;
END;
";

        public const string CreateRtEfluente = @"
BEGIN
    EXECUTE IMMEDIATE '
        CREATE TABLE RT_EFLUENTE (
            ATTACHMENTID NUMBER NOT NULL PRIMARY KEY,
            REL_OBJECTID VARCHAR2(255) NOT NULL,
            CONTENT_TYPE VARCHAR2(255),
            ATT_NAME VARCHAR2(255),
            DATA_SIZE NUMBER,
            DATA BLOB NOT NULL,
            CREATED_AT DATE DEFAULT SYSDATE NOT NULL,
            CONSTRAINT FK_RT_EFLUENTE_PT
                FOREIGN KEY (REL_OBJECTID)
                REFERENCES PT_EFLUENTE(PK_CD_MEIO_AMBIENTE_CPTM)
                ON DELETE CASCADE
        )';
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE != -955 THEN
            RAISE;
        END IF;
END;";

        public const string CreateSequence = @"
BEGIN
    EXECUTE IMMEDIATE 'CREATE SEQUENCE SQ_RT_EFLUENTE START WITH 1 INCREMENT BY 1 NOCACHE';
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE != -955 THEN
            RAISE;
        END IF;
END;";

        public const string AddIsDeletedColumn = @"
BEGIN
    EXECUTE IMMEDIATE 'ALTER TABLE PT_EFLUENTE ADD (IS_DELETED NUMBER(1) DEFAULT 0)';
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE != -1430 THEN
            RAISE;
        END IF;
END;";

        public const string AddDeletedAtColumn = @"
BEGIN
    EXECUTE IMMEDIATE 'ALTER TABLE PT_EFLUENTE ADD (DELETED_AT DATE)';
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE != -1430 THEN
            RAISE;
        END IF;
END;";

        public const string AddDeletedByColumn = @"
BEGIN
    EXECUTE IMMEDIATE 'ALTER TABLE PT_EFLUENTE ADD (DELETED_BY NUMBER)';
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE != -1430 THEN
            RAISE;
        END IF;
END;";

        public const string NormalizeIsDeletedColumn = @"
BEGIN
    EXECUTE IMMEDIATE 'UPDATE PT_EFLUENTE SET IS_DELETED = 0 WHERE IS_DELETED IS NULL';
END;";

        public const string DropSpatialIndexIfExists = @"
DECLARE
    v_count NUMBER;
BEGIN
    SELECT COUNT(*)
    INTO v_count
    FROM USER_INDEXES
    WHERE INDEX_NAME = 'IX_PT_EFLUENTE_GEOM';

    IF v_count > 0 THEN
        EXECUTE IMMEDIATE 'DROP INDEX IX_PT_EFLUENTE_GEOM FORCE';
    END IF;
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE != -1418 THEN
            RAISE;
        END IF;
        NULL;
END;";

        public const string CreateSpatialIndex = @"
BEGIN
    EXECUTE IMMEDIATE 'CREATE INDEX IX_PT_EFLUENTE_GEOM ON PT_EFLUENTE(GEOMETRY) INDEXTYPE IS MDSYS.SPATIAL_INDEX';
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE NOT IN (-955, -29855, -13203) THEN
            RAISE;
        END IF;
END;";
    }
}
