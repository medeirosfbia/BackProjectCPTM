using ApiOracle.Data;
using Dapper;
using System.Threading.Tasks;
using System.Collections.Generic;
using ApiOracle.Models;

namespace ApiOracle.Repositories
{
    public class DominioRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public DominioRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<DominioItem>> ListarItensAsync(string tableName)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<DominioItem>($"SELECT CODIGO, DESCRICAO FROM {tableName} ORDER BY CODIGO");
        }

        public async Task CriarTabelasDominioAsync()
        {
            using var connection = _connectionFactory.CreateConnection();

            // Lista de tabelas e seus dados iniciais baseados no seu arquivo SQL

            await CriarESeed(connection, "GEA_TX_SIGLA_DEPTO_MEIO_AMBIENTE",
                "CREATE TABLE GEA_TX_SIGLA_DEPTO_MEIO_AMBIENTE (CODIGO NUMBER PRIMARY KEY, DESCRICAO VARCHAR2(4000))",
                new List<(int, string)> { 
                    (1, "GEA"), (2, "GEA.DEAE"), (3, "GEA.DEAO"), 
                    (97, "Não se aplica(m)"), (98, "Inexistente(s)"), (99, "Indefinido(a)(s)"), (100, "Não avaliado(a)(s)") 
                });

            await CriarESeed(connection, "GEA_TX_NM_AREA_GESTORA_CPTM",
                "CREATE TABLE GEA_TX_NM_AREA_GESTORA_CPTM (CODIGO NUMBER PRIMARY KEY, DESCRICAO VARCHAR2(4000))",
                new List<(int, string)> {
                    (1, "(DE.GEA.0000) GERENCIA DE MEIO AMBIENTE [ID.10-14-4-0-0000]"),
                    (2, "(DE.GEA.DEAE.0000) DEPTO. DE MEIO AMBIENTE - EMPREENDIMENTOS [ID.10-14-4-1-0000]"),
                    (3, "(DE.GEA.DEAO.0000) DEPTO. DE MEIO AMBIENTE - OPERACAO [ID.10-14-4-2-0000]"),
                    (5, "(DE.GED.0000) GERENCIA DE EMPREENDIMENTOS - EXPANSAO [ID.10-14-7-0-0000]"),
                    (6, "(DE.GED.DEDC.0000) DEPTO. DE OBRAS CIVIS - EXPANSAO [ID.10-14-7-1-0000]"),
                    (7, "(DE.GED.DEDM.0000) DEPTO. DE MONTAGEM DE VIA PERMANENTE E RA [ID.10-14-7-2-0000]"),
                    (8, "(DE.GED.DEDS.0000) DEPTO. DE IMPLANTACAO DE SISTEMAS - EXPANSAO [ID.10-14-7-3-0000]"),
                    (9, "(DE.GEF.0000) GERENCIA DE EMPREENDIMENTOS [ID.10-14-6-0-0000]"),
                    (10, "(DE.GEF.DEFC.0000) DEPTO. DE OBRAS CIVIS [ID.10-14-6-2-0000]"),
                    (11, "(DE.GEF.DEFS.0000) DEPTO. DE OBRAS DE SISTEMAS [ID.10-14-6-1-0000]"),
                    (12, "(DE.GEO.0000) GERENCIA DE EMPREENDIMENTOS - MODERNIZACAO [ID.10-14-2-0-0000]"),
                    (14, "(DE.GEP.0000) GERENCIA DE PROJETOS [ID.10-14-1-0-0000]"),
                    (15, "(DE.GEP.DEPE.0000) DEPTO. DE PROJETOS DE EDIFICACOES [ID.10-14-1-2-0000]"),
                    (16, "(DE.GEP.DEPG.0000) DEPTO. DE CONSISTENCIA E INOVACAO DE PROJETOS [ID.10-14-1-4-0000]"),
                    (17, "(DE.GEP.DEPI.0000) DEPTO. DE PROJETOS DE INFRAESTRUTURA [ID.10-14-1-1-0000]"),
                    (18, "(DE.GEP.DEPS.0000) DEPTO. DE PROJETOS DE INSTALACOES E SISTEMAS [ID.10-14-1-3-0000]"),
                    (19, "(DE.GET.0000) GERENCIA DE EMPREENDIMENTOS - SISTEMAS [ID.10-14-5-0-0000]"),
                    (20, "(DE.GET.DETA.0000) DEPTO. DE SINALIZACAO E TELEFONIA [ID.10-14-5-4-0000]"),
                    (21, "(DE.GET.DETE.0000) DEPTO. DE SISTEMAS DE ENERGIA [ID.10-14-5-5-0000]"),
                    (22, "(DE.GET.DETO.0000) DEPTO. DE PROJETOS DE IMPLANTACAO DE SISTEMAS [ID.10-14-5-6-0000]"),
                    (25, "(DF.GFA.0000) GERENCIA ADMINISTRATIVA [ID.10-12-4-0-0000]"),
                    (29, "(DF.GFA.DFAL.0000) DEPTO. DE LOGISTICA ADMINISTRATIVA [ID.10-12-4-1-0000]"),
                    (34, "(DF.GFA.DFAS.0000) DEPTO. DE SERVICOS ADMINISTRATIVOS [ID.10-12-4-2-0000]"),
                    (326, "(DF.GFH.0000) GERENCIA DE DESENV. ORGAN. E RECURSOS HUMANOS [ID.10-12-8-0-0000]"),
                    (327, "(DF.GFH.DFHS.0000) DEPTO. DE SAUDE E SEG. DO TRABALHO [ID.10-12-8-5-0000]"),
                    (328, "(DF.GFI.0000) GERENCIA DE TECNOLOGIA DA INFORMACAO [ID.10-12-5-0-0000]"),
                    (329, "(DF.GFI.DFIM.0000) DEPTO. DE SUP. E MANUTENCAO DE TI [ID.10-12-5-3-0000]"),
                    (330, "(DO.GOA.0000) GERENCIA DE ATENDIMENTO [ID.10-16-9-0-0000]"),
                    (331, "(DO.GOA.DOAE.0000) DEPTO DE ATENDIMENTO EM ESTACAO [ID.10-16-9-1-0000]"),
                    (332, "(DO.GOA.DOAP.0000) DEPTO DE ATENDIMENTO E SEGURANCA PATRIMONIAL [ID.10-16-9-3-0000]"),
                    (333, "(DO.GOA.DOAS.0000) DEPTO DE ATENDIMENTO E SEGURANCA [ID.10-16-9-2-0000]"),
                    (49, "(DO.GOC.0000) GERENCIA CIRCULACAO E CONTROLE OPERACIONAL [ID.10-16-2-0-0000]"),
                    (334, "(DO.GOC.DOCC.0000) DEPTO. DE CIRCULAÇÃO [ID.10-16-2-8-0000]"),
                    (66, "(DO.GOC.DOCP.0000) DEPTO. DE CONTROLE OPERACIONAL [ID.10-16-2-1-0000]"),
                    (69, "(DO.GOC.DOCT.0000) DEPTO. DE ESTRATEGIA OPERACIONAL [ID.10-16-2-5-0000]"),
                    (131, "(DO.GOF.0000) GERENCIA DE MANUT. DE EQUIPAMENTOS FIXOS [ID.10-15-5-0-0000]"),
                    (132, "(DO.GOF.DOFA.0000) DEPTO. DE MANUT. DE SISTEMAS AUXILIARES [ID.10-15-5-1-0000]"),
                    (142, "(DO.GOF.DOFE.0000) DEPTO. DE MANUT. DE SISTEMAS DE ENERGIA [ID.10-15-5-2-0000]"),
                    (156, "(DO.GOF.DOFS.0000) DEPTO. DE MANUT. DE SISTEMAS ELETR. E RESTAB. DE SERVICOS [ID.10-15-5-3-0000]"),
                    (187, "(DO.GOG.0000) GERENCIA ENG. DE OPERACAO [ID.10-16-7-0-0000]"),
                    (188, "(DO.GOG.DOGC.0000) DEPTO. ENG. DE ESTACOES E COMUNICACAO [ID.10-16-7-2-0000]"),
                    (193, "(DO.GOG.DOGI.0000) DEPTO. ENG. DE SISTEMAS E EQUIPAMENTOS [ID.10-16-7-1-0000]"),
                    (197, "(DO.GOL.0000) GERENCIA DE LOGISTICA [ID.10-15-7-0-0000]"),
                    (198, "(DO.GOL.DOLA.0000) DEPTO. DE ALMOXARIFADOS [ID.10-15-7-1-0000]"),
                    (201, "(DO.GOL.DOLM.0000) DEPTO. DE GESTAO E CADASTRO DE MATERIAIS [ID.10-15-7-2-0000]"),
                    (38, "(DO.GOM.0000) GERENCIA GERAL DE MANUTENCAO [ID.10-15-1-0-0000]"),
                    (39, "(DO.GOO.0000) GERENCIA GERAL DE OPERACAO [ID.10-16-1-0-0000]"),
                    (210, "(DO.GOR.0000) GERENCIA MANUT. MAT RODANTE E OFICINAS [ID.10-15-3-0-0000]"),
                    (213, "(DO.GOR.DORA.0000) DEPTO. MANUT. MAT RODANTE - LAPA [ID.10-15-3-4-0000]"),
                    (219, "(DO.GOR.DORE.0000) DEPTO. MANUT. MAT RODANTE - ENG. S PAULO [ID.10-15-3-6-0000]"),
                    (226, "(DO.GOR.DORO.0000) DEPTO. DE OFICINAS DE MANUT. DE EQUIPAMENTOS [ID.10-15-3-7-0000]"),
                    (241, "(DO.GOR.DORV.0000) DEPTO. MANUT. DE VEICULOS FERROVIARIOS E AUXILIARES [ID.10-15-3-8-0000]"),
                    (274, "(DO.GOT.0000) GERENCIA ENG. DE MANUTENCAO [ID.10-15-4-0-0000]"),
                    (278, "(DO.GOT.DOTI.0000) DEPTO. ENG. DE MANUT. DE INSTALACOES FIXAS [ID.10-15-4-3-0000]"),
                    (281, "(DO.GOT.DOTM.0000) DEPTO. ENG. DE MANUT. DE MAT RODANTE [ID.10-15-4-5-0000]"),
                    (286, "(DO.GOT.DOTV.0000) DEPTO. ENG. DE MANUT. DE VIA PERMANENTE E ESTRUTURA CIVIL [ID.10-15-4-1-0000]"),
                    (289, "(DO.GOV.0000) GERENCIA DE MANUT. DE VIA PERMANENTE E ESTRUTURA CIVIL [ID.10-15-6-0-0000]"),
                    (291, "(DO.GOV.DOVC.0000) DEPTO. DE MANUT. DE ESTRUTURA CIVIL [ID.10-15-6-6-0000]"),
                    (297, "(DO.GOV.DOVF.0000) DEPTO. PLAN. E CONTR. DE MANUT. DE VIA PERMANENTE [ID.10-15-6-4-0000]"),
                    (309, "(DO.GOV.DOVL.0000) DEPTO. DE MANUT. DE VIA PERMANENTE [ID.10-15-6-5-0000]"),
                    (335, "(DP.GPM.DPMT.0000) DEPTO DE GESTÃO DO TERRITÓRIO [ID.10-13-8-2-0000]"),
                    (336, "(DP.GPN.0000) GERENCIA DE NOVOS NEGOCIOS [ID.10-13-4-0-0000]"),
                    (337, "(DP.GPN.DPNG.0000) DEPTO. DE GESTAO DE NEGOCIOS [ID.10-13-4-2-0000]"),
                    (997, "Não se aplica(m)"), (998, "Inexistente(s)"), (999, "Indefinido(a)(s)"), (1000, "Não avaliado(a)(s)")
                });

            await CriarESeed(connection, "GEA_DIR_GER_DEPTO_CPTM",
                "CREATE TABLE GEA_DIR_GER_DEPTO_CPTM (CODIGO NUMBER PRIMARY KEY, DESCRICAO VARCHAR2(4000))",
                new List<(int, string)> {
                    (1, "(DE.GEA.0000) GERENCIA DE MEIO AMBIENTE [ID.10-14-4-0-0000]"),
                    (2, "(DE.GEA.DEAE.0000) DEPTO. DE MEIO AMBIENTE - EMPREENDIMENTOS [ID.10-14-4-1-0000]"),
                    (3, "(DE.GEA.DEAO.0000) DEPTO. DE MEIO AMBIENTE - OPERACAO [ID.10-14-4-2-0000]"),
                    (5, "(DE.GED.0000) GERENCIA DE EMPREENDIMENTOS - EXPANSAO [ID.10-14-7-0-0000]"),
                    (6, "(DE.GED.DEDC.0000) DEPTO. DE OBRAS CIVIS - EXPANSAO [ID.10-14-7-1-0000]"),
                    (7, "(DE.GED.DEDM.0000) DEPTO. DE MONTAGEM DE VIA PERMANENTE E RA [ID.10-14-7-2-0000]"),
                    (8, "(DE.GED.DEDS.0000) DEPTO. DE IMPLANTACAO DE SISTEMAS - EXPANSAO [ID.10-14-7-3-0000]"),
                    (9, "(DE.GEF.0000) GERENCIA DE EMPREENDIMENTOS [ID.10-14-6-0-0000]"),
                    (10, "(DE.GEF.DEFC.0000) DEPTO. DE OBRAS CIVIS [ID.10-14-6-2-0000]"),
                    (11, "(DE.GEF.DEFS.0000) DEPTO. DE OBRAS DE SISTEMAS [ID.10-14-6-1-0000]"),
                    (12, "(DE.GEO.0000) GERENCIA DE EMPREENDIMENTOS - MODERNIZACAO [ID.10-14-2-0-0000]"),
                    (14, "(DE.GEP.0000) GERENCIA DE PROJETOS [ID.10-14-1-0-0000]"),
                    (15, "(DE.GEP.DEPE.0000) DEPTO. DE PROJETOS DE EDIFICACOES [ID.10-14-1-2-0000]"),
                    (16, "(DE.GEP.DEPG.0000) DEPTO. DE CONSISTENCIA E INOVACAO DE PROJETOS [ID.10-14-1-4-0000]"),
                    (17, "(DE.GEP.DEPI.0000) DEPTO. DE PROJETOS DE INFRAESTRUTURA [ID.10-14-1-1-0000]"),
                    (18, "(DE.GEP.DEPS.0000) DEPTO. DE PROJETOS DE INSTALACOES E SISTEMAS [ID.10-14-1-3-0000]"),
                    (19, "(DE.GET.0000) GERENCIA DE EMPREENDIMENTOS - SISTEMAS [ID.10-14-5-0-0000]"),
                    (20, "(DE.GET.DETA.0000) DEPTO. DE SINALIZACAO E TELEFONIA [ID.10-14-5-4-0000]"),
                    (21, "(DE.GET.DETE.0000) DEPTO. DE SISTEMAS DE ENERGIA [ID.10-14-5-5-0000]"),
                    (22, "(DE.GET.DETO.0000) DEPTO. DE PROJETOS DE IMPLANTACAO DE SISTEMAS [ID.10-14-5-6-0000]"),
                    (25, "(DF.GFA.0000) GERENCIA ADMINISTRATIVA [ID.10-12-4-0-0000]"),
                    (29, "(DF.GFA.DFAL.0000) DEPTO. DE LOGISTICA ADMINISTRATIVA [ID.10-12-4-1-0000]"),
                    (34, "(DF.GFA.DFAS.0000) DEPTO. DE SERVICOS ADMINISTRATIVOS [ID.10-12-4-2-0000]"),
                    (326, "(DF.GFH.0000) GERENCIA DE DESENV. ORGAN. E RECURSOS HUMANOS [ID.10-12-8-0-0000]"),
                    (327, "(DF.GFH.DFHS.0000) DEPTO. DE SAUDE E SEG. DO TRABALHO [ID.10-12-8-5-0000]"),
                    (328, "(DF.GFI.0000) GERENCIA DE TECNOLOGIA DA INFORMACAO [ID.10-12-5-0-0000]"),
                    (329, "(DF.GFI.DFIM.0000) DEPTO. DE SUP. E MANUTENCAO DE TI [ID.10-12-5-3-0000]"),
                    (330, "(DO.GOA.0000) GERENCIA DE ATENDIMENTO [ID.10-16-9-0-0000]"),
                    (331, "(DO.GOA.DOAE.0000) DEPTO DE ATENDIMENTO EM ESTACAO [ID.10-16-9-1-0000]"),
                    (332, "(DO.GOA.DOAP.0000) DEPTO DE ATENDIMENTO E SEGURANCA PATRIMONIAL [ID.10-16-9-3-0000]"),
                    (333, "(DO.GOA.DOAS.0000) DEPTO DE ATENDIMENTO E SEGURANCA [ID.10-16-9-2-0000]"),
                    (49, "(DO.GOC.0000) GERENCIA CIRCULACAO E CONTROLE OPERACIONAL [ID.10-16-2-0-0000]"),
                    (334, "(DO.GOC.DOCC.0000) DEPTO. DE CIRCULAÇÃO [ID.10-16-2-8-0000]"),
                    (66, "(DO.GOC.DOCP.0000) DEPTO. DE CONTROLE OPERACIONAL [ID.10-16-2-1-0000]"),
                    (69, "(DO.GOC.DOCT.0000) DEPTO. DE ESTRATEGIA OPERACIONAL [ID.10-16-2-5-0000]"),
                    (131, "(DO.GOF.0000) GERENCIA DE MANUT. DE EQUIPAMENTOS FIXOS [ID.10-15-5-0-0000]"),
                    (132, "(DO.GOF.DOFA.0000) DEPTO. DE MANUT. DE SISTEMAS AUXILIARES [ID.10-15-5-1-0000]"),
                    (142, "(DO.GOF.DOFE.0000) DEPTO. DE MANUT. DE SISTEMAS DE ENERGIA [ID.10-15-5-2-0000]"),
                    (156, "(DO.GOF.DOFS.0000) DEPTO. DE MANUT. DE SISTEMAS ELETR. E RESTAB. DE SERVICOS [ID.10-15-5-3-0000]"),
                    (187, "(DO.GOG.0000) GERENCIA ENG. DE OPERACAO [ID.10-16-7-0-0000]"),
                    (188, "(DO.GOG.DOGC.0000) DEPTO. ENG. DE ESTACOES E COMUNICACAO [ID.10-16-7-2-0000]"),
                    (193, "(DO.GOG.DOGI.0000) DEPTO. ENG. DE SISTEMAS E EQUIPAMENTOS [ID.10-16-7-1-0000]"),
                    (197, "(DO.GOL.0000) GERENCIA DE LOGISTICA [ID.10-15-7-0-0000]"),
                    (198, "(DO.GOL.DOLA.0000) DEPTO. DE ALMOXARIFADOS [ID.10-15-7-1-0000]"),
                    (201, "(DO.GOL.DOLM.0000) DEPTO. DE GESTAO E CADASTRO DE MATERIAIS [ID.10-15-7-2-0000]"),
                    (38, "(DO.GOM.0000) GERENCIA GERAL DE MANUTENCAO [ID.10-15-1-0-0000]"),
                    (39, "(DO.GOO.0000) GERENCIA GERAL DE OPERACAO [ID.10-16-1-0-0000]"),
                    (210, "(DO.GOR.0000) GERENCIA MANUT. MAT RODANTE E OFICINAS [ID.10-15-3-0-0000]"),
                    (213, "(DO.GOR.DORA.0000) DEPTO. MANUT. MAT RODANTE - LAPA [ID.10-15-3-4-0000]"),
                    (219, "(DO.GOR.DORE.0000) DEPTO. MANUT. MAT RODANTE - ENG. S PAULO [ID.10-15-3-6-0000]"),
                    (226, "(DO.GOR.DORO.0000) DEPTO. DE OFICINAS DE MANUT. DE EQUIPAMENTOS [ID.10-15-3-7-0000]"),
                    (241, "(DO.GOR.DORV.0000) DEPTO. MANUT. DE VEICULOS FERROVIARIOS E AUXILIARES [ID.10-15-3-8-0000]"),
                    (274, "(DO.GOT.0000) GERENCIA ENG. DE MANUTENCAO [ID.10-15-4-0-0000]"),
                    (278, "(DO.GOT.DOTI.0000) DEPTO. ENG. DE MANUT. DE INSTALACOES FIXAS [ID.10-15-4-3-0000]"),
                    (281, "(DO.GOT.DOTM.0000) DEPTO. ENG. DE MANUT. DE MAT RODANTE [ID.10-15-4-5-0000]"),
                    (286, "(DO.GOT.DOTV.0000) DEPTO. ENG. DE MANUT. DE VIA PERMANENTE E ESTRUTURA CIVIL [ID.10-15-4-1-0000]"),
                    (289, "(DO.GOV.0000) GERENCIA DE MANUT. DE VIA PERMANENTE E ESTRUTURA CIVIL [ID.10-15-6-0-0000]"),
                    (291, "(DO.GOV.DOVC.0000) DEPTO. DE MANUT. DE ESTRUTURA CIVIL [ID.10-15-6-6-0000]"),
                    (297, "(DO.GOV.DOVF.0000) DEPTO. PLAN. E CONTR. DE MANUT. DE VIA PERMANENTE [ID.10-15-6-4-0000]"),
                    (309, "(DO.GOV.DOVL.0000) DEPTO. DE MANUT. DE VIA PERMANENTE [ID.10-15-6-5-0000]"),
                    (335, "(DP.GPM.DPMT.0000) DEPTO DE GESTÃO DO TERRITÓRIO [ID.10-13-8-2-0000]"),
                    (336, "(DP.GPN.0000) GERENCIA DE NOVOS NEGOCIOS [ID.10-13-4-0-0000]"),
                    (337, "(DP.GPN.DPNG.0000) DEPTO. DE GESTAO DE NEGOCIOS [ID.10-13-4-2-0000]"),
                    (997, "Não se aplica(m)"), (998, "Inexistente(s)"), (999, "Indefinido(a)(s)"), (1000, "Não avaliado(a)(s)")
                });

            await CriarESeed(connection, "GEA_TX_NATUREZA_DO_PGA",
                "CREATE TABLE GEA_TX_NATUREZA_DO_PGA (CODIGO NUMBER PRIMARY KEY, DESCRICAO VARCHAR2(4000))",
                new List<(int, string)> {
                    (13, "Áreas Ambientalmente Protegidas"), (3, "Áreas Contaminadas"), (1, "Arqueologia"),
                    (18, "Comunicação Social"), (17, "Documentação"), (5, "Efluente"),
                    (4, "Emissões Atmosféricas"), (8, "Erosões e Movimentos de Massa"), (11, "Fauna"),
                    (10, "Gerenciamento de Solo"), (16, "Lançamentos Irregulares"), (2, "Patrimônio Histórico"),
                    (6, "Produtos Perigosos"), (15, "Recursos Hídricos"), (7, "Resíduos Sólidos"),
                    (14, "Ruído e Vibração"), (20, "Segmentação Urbana"), (19, "Sinalização e Isolamento"),
                    (9, "Sistema de Drenagem, Inundações e Alagamentos"), (12, "Vegetação"),
                    (97, "Não se aplica(m)"), (98, "Inexistente(s)"), (99, "Indefinido(a)(s)"), (100, "Não avaliado(a)(s)")
                });

            await CriarESeed(connection, "GEA_TX_STATUS_DO_DESVIO_AMBIENTAL",
                "CREATE TABLE GEA_TX_STATUS_DO_DESVIO_AMBIENTAL (CODIGO NUMBER PRIMARY KEY, DESCRICAO VARCHAR2(4000))",
                new List<(int, string)> {
                    (1, "Não Regularizado"), (2, "Regularizado"), 
                    (97, "Não se aplica(m)"), (98, "Inexistente(s)"), (99, "Indefinido(a)(s)"), (100, "Não avaliado(a)(s)")
                });

            await CriarESeed(connection, "GEA_TX_STATUS_DO_REGISTRO_NO_BD",
                "CREATE TABLE GEA_TX_STATUS_DO_REGISTRO_NO_BD (CODIGO NUMBER PRIMARY KEY, DESCRICAO VARCHAR2(4000))",
                new List<(int, string)> {
                    (1, "Ativo"), (2, "Inativo"), 
                    (97, "Não se aplica(m)"), (98, "Inexistente(s)"), (99, "Indefinido(a)(s)"), (100, "Não avaliado(a)(s)")
                });

            await CriarESeed(connection, "GEA_TX_MUNICIPIO",
                "CREATE TABLE GEA_TX_MUNICIPIO (CODIGO NUMBER PRIMARY KEY, DESCRICAO VARCHAR2(4000))",
                new List<(int, string)> {
                    (24, "Arujá"), (10, "Barueri"), (25, "Biritiba-Mirim"), (6, "Caieiras"), (26, "Cajamar"),
                    (4, "Campo Limpo Paulista"), (11, "Carapicuíba"), (27, "Cotia"), (28, "Diadema"), (29, "Embú"),
                    (30, "Embú-Guaçu"), (20, "Ferraz de Vasconcelos"), (5, "Francisco Morato"), (7, "Franco da Rocha"),
                    (31, "Guararema"), (23, "Guarulhos"), (32, "Itapecerica da Serra"), (8, "Itapevi"), (21, "Itaquaquecetuba"),
                    (9, "Jandira"), (2, "Jundiaí"), (33, "Juqutiba"), (46, "Mairinque"), (34, "Mairiporã"),
                    (16, "Mauá"), (18, "Mogi das Cruzes"), (12, "Osasco"), (35, "Pirapora do Bom Jesus"), (22, "Poá"),
                    (14, "Ribeirão Pires"), (13, "Rio Grande da Serra"), (36, "Salesópolis"), (37, "Santa Isabel"),
                    (38, "Santana de Parnaíba"), (15, "Santo André"), (45, "Santos"), (39, "São Bernardo do Campo"),
                    (17, "São Caetano do Sul"), (40, "São Lourenço da Serra"), (1, "São Paulo"), (43, "São Roque"),
                    (44, "São Vicente"), (19, "Suzano"), (41, "Taboão da Serra"), (42, "Vargem Grande Paulista"),
                    (3, "Várzea Paulista"), (99, "Diversos (Ver Observação)"),
                    (997, "Não se aplica(m)"), (998, "Inexistente(s)"), (999, "Indefinido(a)(s)"), (1000, "Não avaliado(a)(s)")
                });

            await CriarESeed(connection, "GEA_TX_LINHA_CPTM",
                "CREATE TABLE GEA_TX_LINHA_CPTM (CODIGO NUMBER PRIMARY KEY, DESCRICAO VARCHAR2(4000))",
                new List<(int, string)> {
                    (1, "Linha 07 - Rubi"), (2, "Linha 08 - Diamante"), (3, "Linha 09 - Esmeralda"),
                    (4, "Linha 10 - Turquesa"), (5, "Linha 11 - Coral"), (6, "Linha 12 - Safira"),
                    (7, "Linha 07 - Rubi / Linha 08 - Diamante"), (9, "Linha 09 - Esmeralda / Linha 10 - Turquesa"),
                    (8, "Linha 08 - Diamante / Linha 09 - Esmeralda"), (10, "Linha 07 - Rubi / Linha 08 - Diamante / Linha 11 - Coral"),
                    (11, "Linha 07 - Rubi / Linha 11 - Coral"), (12, "Linha 10 - Turquesa / Linha 11 - Coral"),
                    (13, "Linha 11 - Coral / Linha 12 - Safira"), (14, "Linha 10 - Turquesa / Linha 11 - Coral / Linha 12 - Safira"),
                    (15, "Linha 07 - Rubi / Linha 08 - Diamante / Linha 09 - Esmeralda / Linha 10 - Turquesa / Linha 11 - Coral"),
                    (16, "Linha 08 - Diamante / Linha 09 - Esmeralda / Linha 10 - Turquesa / Linha 11 - Coral / Linha 12 - Safira"),
                    (17, "Linha 07 - Rubi / Linha 08 - Diamante / Linha 10 - Turquesa / Linha 11 - Coral / Linha 12 - Safira"),
                    (18, "Sem linha associada"), (19, "Linha 07 - Rubi / Linha 08 - Diamante / Linha 09 - Esmeralda / Linha 10 - Turquesa / Linha 11 - Coral / Linha 12 - Safira"),
                    (20, "Linha 13 - Jade"), (21, "Linha 05 - Lilás"), (22, "Linha 11 - Coral / Linha 12 - Safira / 13 - Jade"),
                    (23, "Linha 10 - Turquesa / Linha 11 - Coral / Linha 12 - Safira / Linha 13 - Jade"), (24, "Linha 07 - Rubi / Linha 10 - Turquesa"),
                    (25, "Linha JJ - Baixada Santista"), (26, "Linha 09 - Esmeralda / Linha 05 - Lilás"),
                    (27, "Linha 07 - Rubi / Linha 08 - Diamante / Linha 09 - Esmeralda / Linha 12 - Safira"), (28, "Linha 07 - Rubi / Linha 12 - Safira"),
                    (29, "Linha 07 - Rubi / Linha 09 - Esmeralda"), (30, "Linha 08 - Diamante / Linha 10 - Turquesa / Linha 11 - Coral / Linha 12 - Safira"),
                    (31, "Linha 08 - Diamante / Linha 09 - Esmeralda / Linha 10 - Turquesa / Linha 11 - Coral / Linha 12 - Safira / Linha JJ - Baixada Santista"),
                    (32, "Linha não informada"), (33, "Linha 12 - Safira/ Linha 13 - Jade"),
                    (97, "Não se aplica(m)"), (98, "Inexistente(s)"), (99, "Indefinido(a)(s)"), (100, "Não avaliado(a)(s)")
                });

            await CriarESeed(connection, "GEA_TX_VIA_CPTM",
                "CREATE TABLE GEA_TX_VIA_CPTM (CODIGO NUMBER PRIMARY KEY, DESCRICAO VARCHAR2(4000))",
                new List<(int, string)> {
                    (1, "Via 01"), (2, "Via 02"), (3, "Via 03"), (4, "Via 04"), (5, "Via 05"),
                    (6, "Via 06"), (7, "Via 08"), (8, "Via 09"), (9, "Via 10"), (10, "Via 01S - Trecho 1"),
                    (11, "Via 01S - Trecho 2"), (12, "Via 02S - Trecho 1"), (13, "Via 02S - Trecho 2"),
                    (14, "Via 03S - Trecho 2"), (15, "Via 03E - Trecho 2"), (16, "Via 04E - Trecho 2"),
                    (17, "Via Auxiliar"), (18, "Via Variante"), (19, "Travessão - AMV"),
                    (97, "Não se aplica(m)"), (98, "Inexistente(s)"), (99, "Indefinido(a)(s)"), (100, "Não avaliado(a)(s)")
                });

            await CriarESeed(connection, "GEA_TX_TRECHO_E_SENTIDO_CPTM",
                "CREATE TABLE GEA_TX_TRECHO_E_SENTIDO_CPTM (CODIGO NUMBER PRIMARY KEY, DESCRICAO VARCHAR2(4000))",
                new List<(int, string)> {
                    (62, "Estação Aeroporto Guarulhos - Estação Guarulhos - Cecap"), (126, "Estação Aeroporto Guarulhos - Final dos Trilhos"),
                    (38, "Estação Água Branca - Estação Lapa"), (40, "Estação Água Branca - Estação Lapa (Linha 07)"),
                    (118, "Estação Água Branca - Estação Palmeiras - Barra Funda"), (107, "Estação Antônio Gianetti Neto - Estação Ferraz de Vasconcelos"),
                    (41, "Estação Antônio Gianetti Neto - Estação Guaianazes"), (84, "Estação Aracaré - Estação Calmon Viana"),
                    (48, "Estação Aracaré - Estação de Itaquaquecetuba"), (87, "Estação Baltazar Fidelis - Estação de Franco da Rocha"),
                    (23, "Estação Baltazar Fidelis - Estação Francisco Morato"), (117, "Estação Barra Funda - Estação Luz"),
                    (51, "Estação Botujuru - Estação Campo Limpo Paulista"), (85, "Estação Botujuru - Estação Francisco Morato"),
                    (13, "Estação Brás Cubas - Estação Jundiapeba"), (77, "Estação Brás Cubas - Estação Mogi das Cruzes"),
                    (25, "Estação Caieiras - Estação Franco da Rocha"), (89, "Estação Caieiras - Estação Perus"),
                    (21, "Estação Calmon Viana - Estação Aracaré"), (45, "Estação Calmon Viana - Estação Poá"),
                    (74, "Estação Calmon Viana - Estação Suzano"), (114, "Estação Campo Limpo Paulista - Estação Botujuru"),
                    (50, "Estação Campo Limpo Paulista - Estação Várzea Paulista"), (67, "Estação Capuava - Estação Mauá"),
                    (3, "Estação Capuava - Estação Santo André"), (81, "Estação Com. Ermelino Matarazzo - Estação São Miguel Paulista"),
                    (58, "Estação Comendador Ermelino Matarazzo - Estação USP Leste"), (120, "Estação Corinthians - Itaquera - Estação Dom Bosco"),
                    (9, "Estação Corinthians - Itaquera - Estação Tatuapé"), (97, "Estação da Mooca - Estação Ipiranga"),
                    (33, "Estação da Mooca - Estação Roosevelt/Brás"), (24, "Estação de Franco da Rocha - Estação Baltazar Fidelis"),
                    (111, "Estação de Itaquaquecetuba - Estação Aracaré"), (47, "Estação de Itaquaquecetuba - Estação Engenheiro Manoel Feio"),
                    (8, "Estação de Paranapiacaba - Estação Rio Grande da Serra"), (106, "Estação de Paranapiacaba - Final dos Trilhos"),
                    (57, "Estação Dom Bosco - Estação Corinthians - Itaquera"), (73, "Estação Dom Bosco - Estação José Bonifácio"),
                    (124, "Estação Engenheiro Goulart - Estação Guarulhos - Cecap"), (20, "Estação Engenheiro Goulart - Estação Tatuapé"),
                    (82, "Estação Engenheiro Goulart - Estação USP Leste"), (110, "Estação Engenheiro Manoel Feio - Estação de Itaquaquecetuba"),
                    (60, "Estação Engenheiro Manoel Feio - Estação Jardim Romano"), (15, "Estação Estudantes - Estação Mogi das Cruzes"),
                    (102, "Estação Estudantes - Final dos Trilhos"), (44, "Estação Ferraz de Vasconcelos - Estação Antônio Gianetti Neto"),
                    (109, "Estação Ferraz de Vasconcelos - Estação Poá"), (86, "Estação Francisco Morato - Estação Baltazar Fidelis"),
                    (22, "Estação Francisco Morato - Estação Botujuru"), (88, "Estação Franco da Rocha - Estação Caieiras"),
                    (104, "Estação Guaianazes - Estação Antônio Gianetti Neto"), (42, "Estação Guaianazes - Estação José Bonifácio"),
                    (5, "Estação Guapituba - Estação Mauá"), (69, "Estação Guapituba - Estação Ribeirão Pires"),
                    (125, "Estação Guarulhos - Cecap - Estação Aeroporto Guarulhos"), (61, "Estação Guarulhos - Cecap - Estação Engenheiro Goulart"),
                    (34, "Estação Ipiranga - Estação da Mooca"), (98, "Estação Ipiranga - Estação Tamanduateí"),
                    (59, "Estação Itaim Paulista - Estação Jardim Helena - Vila Mara"), (79, "Estação Itaim Paulista - Estação Jardim Romano"),
                    (56, "Estação Jaraguá - Estação Vila Aurora"), (91, "Estação Jaraguá - Estação Vila Clarisse"),
                    (122, "Estação Jardim Helena - Vila Mara - Estação Itaim Paulista"), (17, "Estação Jardim Helena - Vila Mara - Estação São Miguel Paulista"),
                    (123, "Estação Jardim Romano - Estação Engenheiro Manoel Feio"), (16, "Estação Jardim Romano - Estação Itaim Paulista"),
                    (10, "Estação José Bonifácio - Estação Dom Bosco"), (105, "Estação José Bonifácio - Estação Guaianazes"),
                    (116, "Estação Jundiaí - Estação Várzea Paulista"), (52, "Estação Jundiaí - Final dos Trilhos"),
                    (76, "Estação Jundiapeba - Estação Brás Cubas"), (12, "Estação Jundiapeba - Estação Suzano"),
                    (101, "Estação Lapa - Estação Água Branca"), (103, "Estação Lapa (Linha 07) - Estação Água Branca"),
                    (31, "Estação Lapa (Linha 07) - Estação Piqueri"), (54, "Estação Luz - Estação Barra Funda"),
                    (95, "Estação Luz - Estação Roosevelt/Brás"), (4, "Estação Mauá - Estação Capuava"),
                    (68, "Estação Mauá - Estação Guapituba"), (14, "Estação Mogi das Cruzes - Estação Brás Cubas"),
                    (78, "Estação Mogi das Cruzes - Estação Estudantes"), (55, "Estação Palmeiras - Barra Funda - Estação Água Branca"),
                    (26, "Estação Perus - Estação Caieiras"), (90, "Estação Perus - Estação Vila Aurora"),
                    (94, "Estação Piqueri - Estação Lapa (Linha 07)"), (30, "Estação Piqueri - Estação Pirituba"),
                    (93, "Estação Pirituba - Estação Piqueri"), (29, "Estação Pirituba - Estação Vila Clarisse"),
                    (108, "Estação Poá - Estação Calmon Viana"), (46, "Estação Poá - Estação Ferraz de Vasconcelos"),
                    (65, "Estação Prefeito Saladino - Estação Santo André"), (1, "Estação Prefeito Saladino - Estação Utinga"),
                    (6, "Estação Ribeirão Pires - Estação Guapituba"), (70, "Estação Ribeirão Pires - Estação Rio Grande da Serra"),
                    (71, "Estação Rio Grande da Serra - Estação de Paranapiacaba"), (7, "Estação Rio Grande da Serra - Estação Ribeirão Pires"),
                    (96, "Estação Roosevelt/Brás - Estação da Mooca"), (32, "Estação Roosevelt/Brás - Estação Luz"),
                    (112, "Estação Roosevelt/Brás - Estação Tatuapé"), (66, "Estação Santo André - Estação Capuava"),
                    (2, "Estação Santo André - Estação Prefeito Saladino"), (36, "Estação São Caetano - Estação Tamanduateí"),
                    (100, "Estação São Caetano - Estação Utinga"), (18, "Estação São Miguel Paulista - Estação Com. Ermelino Matarazzo"),
                    (80, "Estação São Miguel Paulista - Estação Jardim Helena - Vila Mara"), (11, "Estação Suzano - Estação Calmon Viana"),
                    (75, "Estação Suzano - Estação Jundiapeba"), (35, "Estação Tamanduateí - Estação Ipiranga"),
                    (99, "Estação Tamanduateí - Estação São Caetano"), (72, "Estação Tatuapé - Estação Corinthians - Itaquera"),
                    (83, "Estação Tatuapé - Estação Engenheiro Goulart"), (49, "Estação Tatuapé - Estação Roosevelt/Brás"),
                    (121, "Estação USP Leste - Estação Comendador Ermelino Matarazzo"), (19, "Estação USP Leste - Estação Engenheiro Goulart"),
                    (64, "Estação Utinga - Estação Prefeito Saladino"), (37, "Estação Utinga - Estação São Caetano"),
                    (113, "Estação Várzea Paulista - Estação Campo Limpo Paulista"), (53, "Estação Várzea Paulista - Estação Jundiaí"),
                    (119, "Estação Vila Aurora - Estação Jaraguá"), (27, "Estação Vila Aurora - Estação Perus"),
                    (28, "Estação Vila Clarisse - Estação Jaraguá"), (92, "Estação Vila Clarisse - Estação Pirituba"),
                    (63, "Final dos Trilhos - Estação Aeroporto Guarulhos"), (43, "Final dos Trilhos - Estação de Paranapiacaba"),
                    (39, "Final dos Trilhos - Estação Estudantes"), (115, "Final dos Trilhos - Estação Jundiaí"),
                    (997, "Não se aplica(m)"), (998, "Inexistente(s)"), (999, "Indefinido(a)(s)"), (1000, "Não avaliado(a)(s)")
                });

            await CriarESeed(connection, "GEA_TX_ESTACAO_CPTM",
                "CREATE TABLE GEA_TX_ESTACAO_CPTM (CODIGO NUMBER PRIMARY KEY, DESCRICAO VARCHAR2(4000))",
                new List<(int, string)> {
                    (1, "Estação Aeroporto Guarulhos"), (2, "Estação Água Branca"), (3, "Estação Antonio Gianetti Neto"),
                    (4, "Estação Antônio João"), (5, "Estação Aracaré"), (6, "Estação Autódromo"), (7, "Estação Baltazar Fidelis"),
                    (8, "Estação Barueri"), (9, "Estação Berrini"), (10, "Estação Botujuru"), (11, "Estação Brás"),
                    (12, "Estação Brás Cubas"), (13, "Estação Caieiras"), (14, "Estação Calmon Viana"), (15, "Estação Campo Limpo Paulista"),
                    (16, "Estação Capuava"), (17, "Estação Carapicuíba"), (18, "Estação Ceasa"), (19, "Estação Cidade Jardim"),
                    (20, "Estação Cidade Universitária"), (21, "Estação Comandante Sampaio"), (22, "Estação Comendador Ermelino"),
                    (23, "Estação Corinthians - Itaquera"), (24, "Estação Dom Bosco"), (25, "Estação Domingos de Morais"),
                    (26, "Estação Engenheiro Cardoso"), (27, "Estação Engenheiro Goulart"), (28, "Estação Engenheiro Manoel Feio"),
                    (29, "Estação Estudantes"), (30, "Estação Ferraz de Vasconcelos"), (31, "Estação Francisco Morato"),
                    (32, "Estação Franco da Rocha"), (33, "Estação General Miguel Costa"), (34, "Estação Grajaú"),
                    (35, "Estação Granja Julieta"), (36, "Estação Guaianazes"), (37, "Estação Guapituba"),
                    (38, "Estação Guarulhos Cecap"), (39, "Estação Hebraica - Rebouças"), (40, "Estação Imperatriz Leopoldina"),
                    (41, "Estação Ipiranga"), (42, "Estação Itaim Paulista"), (43, "Estação Itapevi"), (44, "Estação Itaquaquecetuba"),
                    (45, "Estação Jandira"), (46, "Estação Jaraguá"), (47, "Estação Jardim Belval"),
                    (48, "Estação Jardim Helena - Vila Mara"), (49, "Estação Jardim Romano"), (50, "Estação Jardim Silveira"),
                    (51, "Estação João Dias"), (52, "Estação José Bonifácio"), (53, "Estação Júlio Prestes"),
                    (54, "Estação Jundiaí"), (55, "Estação Jundiapeba"), (56, "Estação Jurubatuba"), (57, "Estação Lapa (Linha 7)"),
                    (58, "Estação Lapa (Linha 8)"), (59, "Estação Luz"), (60, "Estação Mauá"), (61, "Estação Mendes / Bruno Covas"),
                    (62, "Estação Mogi das Cruzes"), (63, "Estação Móoca"), (64, "Estação Morumbi"), (65, "Estação Osasco"),
                    (66, "Estação Palmeiras - Barra Funda"), (67, "Estação Parada Amador Bueno"), (68, "Estação Perus"),
                    (69, "Estação Pinheiros"), (70, "Estação Piqueri"), (71, "Estação Pirituba"), (72, "Estação Poá"),
                    (73, "Estação Prefeito Celso Daniel - Santo André"), (74, "Estação Prefeito Saladino"), (75, "Estação Presidente Altino"),
                    (76, "Estação Primavera - Interlagos"), (77, "Estação Quitaúna"), (78, "Estação Ribeirão Pires"),
                    (79, "Estação Rio Grande da Serra"), (80, "Estação Sagrado Coração"), (81, "Estação Santa Rita"),
                    (82, "Estação Santa Terezinha"), (83, "Estação Santo Amaro (Linha 9)"), (84, "Estação São Caetano"),
                    (85, "Estação São Miguel Paulista"), (86, "Estação Socorro"), (87, "Estação Suzano"),
                    (88, "Estação Tamanduateí"), (89, "Estação Tatuapé"), (90, "Estação USP Leste"), (91, "Estação Utinga"),
                    (92, "Estação Várzea Paulista"), (93, "Estação Vila Aurora"), (94, "Estação Vila Clarice"),
                    (95, "Estação Vila Olímpia"), (96, "Estação Villa-Lobos - Jaguaré"),
                    (997, "Não se aplica(m)"), (998, "Inexistente(s)"), (999, "Indefinido(a)(s)"), (1000, "Não avaliado(a)(s)")
                });

            await CriarESeed(connection, "TIPO_PROPRIETARIO",
                "CREATE TABLE TIPO_PROPRIETARIO (CODIGO NUMBER PRIMARY KEY, DESCRICAO VARCHAR2(4000))",
                new List<(int, string)> {
                    (1, "CPTM - Titularidade"), (2, "CPTM - Posse"), (3, "Metrô"), (4, "Alienado"), (5, "MRS"),
                    (6, "RFSA"), (7, "RFSA/SPU"), (8, "CBTU"), (9, "Pessoa Jurídica"), (10, "Pessoa Física"),
                    (11, "Indefinido"), (13, "FEPASA"), (14, "Permuta")
                });

            await CriarESeed(connection, "TIPO_PROPRIETARIO_L13",
                "CREATE TABLE TIPO_PROPRIETARIO_L13 (CODIGO NUMBER PRIMARY KEY, DESCRICAO VARCHAR2(4000))",
                new List<(int, string)> {
                    (1, "CPTM - Titularidade"), (5, "MRS"), (15, "Prefeitura de Guarulhos"), (16, "DAEE"),
                    (18, "USP Leste"), (19, "GRU - Aeroporto"), (20, "CCR - Rodovia Dutra"), (21, "Ecopistas"), (22, "CDHU")
                });

            await CriarESeed(connection, "GEA_TX_PROPRIETARIO",
                "CREATE TABLE GEA_TX_PROPRIETARIO (CODIGO NUMBER PRIMARY KEY, DESCRICAO VARCHAR2(4000))",
                new List<(int, string)> {
                    (1, "CPTM - Titularidade"), (2, "CPTM - Posse"), (3, "Metrô"), (4, "Alienado"), (5, "MRS"),
                    (6, "RFSA"), (7, "RFSA/SPU"), (8, "CBTU"), (9, "Pessoa Jurídica"), (10, "Pessoa Física"),
                    (11, "Indefinido"), (13, "FEPASA"), (14, "Permuta"), (15, "Prefeitura de Guarulhos"),
                    (16, "DAEE"), (18, "USP Leste"), (19, "GRU - Aeroporto"), (20, "CCR - Rodovia Dutra"),
                    (21, "Ecopistas"), (22, "CDHU"),
                    (97, "Não se aplica(m)"), (98, "Inexistente(s)"), (100, "Não avaliado(a)(s)")
                });

            await CriarESeed(connection, "GEA_SIM_NÃO",
                "CREATE TABLE GEA_SIM_NÃO (CODIGO NUMBER PRIMARY KEY, DESCRICAO VARCHAR2(4000))",
                new List<(int, string)> {
                    (1, "Sim"), (2, "Não"), (3, "Não Informado"), 
                    (97, "Não se aplica(m)"), (98, "Inexistente(s)"), (99, "Indefinido(a)(s)"), (100, "Não avaliado(a)(s)")
                });

            await CriarESeed(connection, "EF_TX_TIPO_ATIVIDADE_LISTADA",
                "CREATE TABLE EF_TX_TIPO_ATIVIDADE_LISTADA (CODIGO NUMBER PRIMARY KEY, DESCRICAO VARCHAR2(4000))",
                new List<(int, string)> {
                    (1, "Estação de Tratamento de Efluente"), (2, "Transporte"), 
                    (96, "Outro(a)(s)"), (99, "Indefinido(a)(s)"), (97, "Não se aplica(m)"), 
                    (98, "Inexistente(s)"), (100, "Não avaliado(a)(s)")
                });

            await CriarESeed(connection, "EF_TX_TIPO_DRA_LISTADO",
                "CREATE TABLE EF_TX_TIPO_DRA_LISTADO (CODIGO NUMBER PRIMARY KEY, DESCRICAO VARCHAR2(4000))",
                new List<(int, string)> {
                    (1, "Cadastro Técnico Federal (IBAMA) - CTF/IBAMA"), (2, "Certificado de Dispensa de Licença - CDL"),
                    (3, "Certificado de Movimentação de Resíduos de Interesse Ambiental - CADRI"),
                    (4, "Declaração de Movimentação de Resíduos - DMR"),
                    (5, "Ficha de Informações de Segurança de Produtos Químicos - FISPQ"),
                    (6, "Licença de Operação - LO"), (7, "Manifesto de Transporte de Resíduos - MTR"),
                    (96, "Outro(a)(s)"), (99, "Indefinido(a)(s)"), (97, "Não se aplica(m)"), 
                    (98, "Inexistente(s)"), (100, "Não avaliado(a)(s)")
                });

            await CriarESeed(connection, "EF_TX_TIPO_ATIVIDADE_CPTM",
                "CREATE TABLE EF_TX_TIPO_ATIVIDADE_CPTM (CODIGO NUMBER PRIMARY KEY, DESCRICAO VARCHAR2(4000))",
                new List<(int, string)> {
                    (1, "Empreendimento/Obra"), (2, "Manutenção"), (3, "Operação"),
                    (96, "Outro(a)(s)"), (99, "Indefinido(a)(s)"), (97, "Não se aplica(m)"), 
                    (98, "Inexistente(s)"), (100, "Não avaliado(a)(s)")
                });

            await CriarESeed(connection, "EF_TX_NM_LOCAL_ATIV",
                "CREATE TABLE EF_TX_NM_LOCAL_ATIV (CODIGO NUMBER PRIMARY KEY, DESCRICAO VARCHAR2(4000))",
                new List<(int, string)> {
                    (1, "Abrigo"), (2, "Base de manutenção"), (3, "Cabine Primária"), (4, "Cabine Seccionadora"),
                    (5, "Estação"), (6, "Lavador de TUE"), (7, "Oficina"), (8, "Pátio"), (9, "Prédio administrativo"),
                    (10, "Prédio de apoio"), (11, "Sala técnica"), (12, "Subestação"), (13, "Trecho - Km/poste"), (14, "Vários"),
                    (96, "Outro(a)(s)"), (99, "Indefinido(a)(s)"), (97, "Não se aplica(m)"), 
                    (98, "Inexistente(s)"), (100, "Não avaliado(a)(s)")
                });

            await CriarESeed(connection, "EF_TX_ORIGEM_EFLUENTE",
                "CREATE TABLE EF_TX_ORIGEM_EFLUENTE (CODIGO NUMBER PRIMARY KEY, DESCRICAO VARCHAR2(4000))",
                new List<(int, string)> {
                    (1, "Doméstico/Sanitário"), (2, "Fundação"), (3, "Industrial"),
                    (96, "Outro(a)(s)"), (99, "Indefinido(a)(s)"), (97, "Não se aplica(m)"), 
                    (98, "Inexistente(s)"), (100, "Não avaliado(a)(s)")
                });

            await CriarESeed(connection, "EF_TX_FONTE_GERADORA",
                "CREATE TABLE EF_TX_FONTE_GERADORA (CODIGO NUMBER PRIMARY KEY, DESCRICAO VARCHAR2(4000))",
                new List<(int, string)> {
                    (1, "Atividade de obra"), (2, "Banheiro químico"), (3, "Banheiros/vestiários/refeitórios"),
                    (4, "Fossa séptica"), (5, "Lavagem de trens/peças"), (6, "Manutenção ETE"), (7, "Valas de manutenção"),
                    (96, "Outro(a)(s)"), (99, "Indefinido(a)(s)"), (97, "Não se aplica(m)"), 
                    (98, "Inexistente(s)"), (100, "Não avaliado(a)(s)")
                });

            await CriarESeed(connection, "EF_TX_TIPO_DESTINACAO",
                "CREATE TABLE EF_TX_TIPO_DESTINACAO (CODIGO NUMBER PRIMARY KEY, DESCRICAO VARCHAR2(4000))",
                new List<(int, string)> {
                    (1, "Esgotamento e transporte"), (2, "Interligação em rede coletora"), 
                    (3, "Lançamento em galeria de águas pluviais"), (4, "Reinfiltração"), (5, "Tratamento em ETE"),
                    (96, "Outro(a)(s)"), (99, "Indefinido(a)(s)"), (97, "Não se aplica(m)"), 
                    (98, "Inexistente(s)"), (100, "Não avaliado(a)(s)")
                });

            await CriarESeed(connection, "EF_TX_TIPO_VEICULO",
                "CREATE TABLE EF_TX_TIPO_VEICULO (CODIGO NUMBER PRIMARY KEY, DESCRICAO VARCHAR2(4000))",
                new List<(int, string)> {
                    (1, "Caminhão"), 
                    (96, "Outro(a)(s)"), (99, "Indefinido(a)(s)"), (97, "Não se aplica(m)"), 
                    (98, "Inexistente(s)"), (100, "Não avaliado(a)(s)")
                });

        }

        private async Task CriarESeed(System.Data.IDbConnection conn, string tableName, string createSql, List<(int code, string desc)> seedData)
        {
            // Verifica se a tabela já existe no esquema do usuário
            var checkTableSql = "SELECT COUNT(*) FROM user_tables WHERE table_name = :tableName";
            var tableExists = await conn.ExecuteScalarAsync<int>(checkTableSql, new { tableName = tableName.ToUpper() });

            if (tableExists == 0)
            {
                // Cria a tabela
                await conn.ExecuteAsync(createSql);

                // Popula com dados iniciais (Seed)
                var insertSql = $"INSERT INTO {tableName} (CODIGO, DESCRICAO) VALUES (:code, :description)";
                foreach (var item in seedData)
                {
                    await conn.ExecuteAsync(insertSql, new { code = item.code, description = item.desc });
                }
            }
            else
            {
                // Opcional: Verificar se a tabela está vazia para aplicar o seed mesmo que já exista
                var count = await conn.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM {tableName}");
                if (count == 0)
                {
                    // Aplica o seed se estiver vazia
                }
            }
        }
    }
}
