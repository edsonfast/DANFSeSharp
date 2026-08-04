using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SysValley.Danfse
{
    /// <summary>
    /// Formatacao dos valores do XML para o texto exibido no DANFSe e traducao
    /// dos codigos de dominio do layout nacional da NFS-e.
    /// </summary>
    public static class DanfseFormato
    {
        /// <summary>Texto usado quando o campo nao tem valor no XML.</summary>
        public const string Vazio = "-";

        private static readonly CultureInfo Br = CultureInfo.GetCultureInfo("pt-BR");

        public static string OuVazio(string s)
        {
            return string.IsNullOrWhiteSpace(s) ? Vazio : s.Trim();
        }

        public static string Juntar(string separador, params string[] partes)
        {
            return string.Join(separador, partes.Select(OuVazio));
        }

        private static string SoDigitos(string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            var sb = new StringBuilder(s.Length);
            foreach (char c in s) if (char.IsDigit(c)) sb.Append(c);
            return sb.ToString();
        }

        // ---------------------------------------------------------------- docs

        /// <summary>CNPJ (14) -> 00.000.000/0000-00; CPF (11) -> 000.000.000-00; demais -> como veio.</summary>
        public static string CnpjCpf(string doc)
        {
            string d = SoDigitos(doc);
            if (d.Length == 14)
                return string.Format("{0}.{1}.{2}/{3}-{4}",
                    d.Substring(0, 2), d.Substring(2, 3), d.Substring(5, 3), d.Substring(8, 4), d.Substring(12, 2));
            if (d.Length == 11)
                return string.Format("{0}.{1}.{2}-{3}",
                    d.Substring(0, 3), d.Substring(3, 3), d.Substring(6, 3), d.Substring(9, 2));
            return OuVazio(doc);
        }

        /// <summary>CEP (8) -> 00.000-000 (mascara adotada pelo DANFSe nacional).</summary>
        public static string Cep(string cep)
        {
            string d = SoDigitos(cep);
            if (d.Length != 8) return OuVazio(cep);
            return string.Format("{0}.{1}-{2}", d.Substring(0, 2), d.Substring(2, 3), d.Substring(5, 3));
        }

        /// <summary>Codigo IBGE (7) -> 00.00000.</summary>
        public static string Ibge(string cMun)
        {
            string d = SoDigitos(cMun);
            if (d.Length != 7) return OuVazio(cMun);
            return string.Format("{0}.{1}", d.Substring(0, 2), d.Substring(2, 5));
        }

        /// <summary>Codigo de tributacao nacional (6) -> 00.00.00.</summary>
        public static string CodigoTributacaoNacional(string c)
        {
            string d = SoDigitos(c);
            if (d.Length != 6) return OuVazio(c);
            return string.Format("{0}.{1}.{2}", d.Substring(0, 2), d.Substring(2, 2), d.Substring(4, 2));
        }

        /// <summary>Codigo NBS (9) -> 0.0000.00.00.</summary>
        public static string Nbs(string c)
        {
            string d = SoDigitos(c);
            if (d.Length != 9) return OuVazio(c);
            return string.Format("{0}.{1}.{2}.{3}",
                d.Substring(0, 1), d.Substring(1, 4), d.Substring(5, 2), d.Substring(7, 2));
        }

        /// <summary>Telefone (10/11 digitos) -> (00) 0000-0000 / (00) 00000-0000.</summary>
        public static string Telefone(string fone)
        {
            string d = SoDigitos(fone);
            if (d.Length == 11)
                return string.Format("({0}) {1}-{2}", d.Substring(0, 2), d.Substring(2, 5), d.Substring(7, 4));
            if (d.Length == 10)
                return string.Format("({0}) {1}-{2}", d.Substring(0, 2), d.Substring(2, 4), d.Substring(6, 4));
            if (d.Length == 9 || d.Length == 8)
                return string.Format("{0}-{1}", d.Substring(0, d.Length - 4), d.Substring(d.Length - 4));
            return OuVazio(fone);
        }

        // ------------------------------------------------------------ numeros

        public static string Moeda(decimal? v)
        {
            if (!v.HasValue) return Vazio;
            return "R$ " + v.Value.ToString("N2", Br);
        }

        /// <summary>Moeda que imprime R$ 0,00 no lugar de "-" quando o valor for nulo.</summary>
        public static string MoedaZero(decimal? v)
        {
            return "R$ " + (v ?? 0m).ToString("N2", Br);
        }

        public static string Percentual(decimal? v, int casas = 2)
        {
            if (!v.HasValue) return Vazio;
            return v.Value.ToString("N" + casas.ToString(CultureInfo.InvariantCulture), Br) + "%";
        }

        public static string Data(DateTime? d)
        {
            return d.HasValue ? d.Value.ToString("dd/MM/yyyy", Br) : Vazio;
        }

        public static string DataHora(DateTime? d)
        {
            return d.HasValue ? d.Value.ToString("dd/MM/yyyy HH:mm:ss", Br) : Vazio;
        }

        // ------------------------------------------------------ dominios NFSe

        /// <summary>UF a partir dos 2 primeiros digitos do codigo IBGE do municipio.</summary>
        public static string UfPorCodigoIbge(string cMun)
        {
            string d = SoDigitos(cMun);
            if (d.Length < 2) return null;
            string uf;
            return UfsPorCodigo.TryGetValue(d.Substring(0, 2), out uf) ? uf : null;
        }

        private static readonly Dictionary<string, string> UfsPorCodigo = new Dictionary<string, string>
        {
            { "11", "RO" }, { "12", "AC" }, { "13", "AM" }, { "14", "RR" }, { "15", "PA" },
            { "16", "AP" }, { "17", "TO" }, { "21", "MA" }, { "22", "PI" }, { "23", "CE" },
            { "24", "RN" }, { "25", "PB" }, { "26", "PE" }, { "27", "AL" }, { "28", "SE" },
            { "29", "BA" }, { "31", "MG" }, { "32", "ES" }, { "33", "RJ" }, { "35", "SP" },
            { "41", "PR" }, { "42", "SC" }, { "43", "RS" }, { "50", "MS" }, { "51", "MT" },
            { "52", "GO" }, { "53", "DF" }
        };

        public static string AmbienteGerador(string c)
        {
            switch (c)
            {
                case "1": return "1";   // Prefeitura
                case "2": return "2";   // SEFIN Nacional
                default: return OuVazio(c);
            }
        }

        public static string TipoEmitente(string tpEmit)
        {
            switch (tpEmit)
            {
                case "1": return "Prestador";
                case "2": return "Tomador";
                case "3": return "Intermediário";
                default: return Vazio;
            }
        }

        public static string OpcaoSimplesNacional(string opSimpNac)
        {
            switch (opSimpNac)
            {
                case "1": return "Não Optante";
                case "2": return "Optante - Microempreendedor Individual (MEI)";
                case "3": return "Optante - Microempresa ou Empresa de Pequeno Porte (ME/EPP)";
                default: return Vazio;
            }
        }

        public static string RegimeApuracaoSn(string regApTribSN)
        {
            switch (regApTribSN)
            {
                case "1": return "Regime de apuração dos tributos federais e municipal pelo SN";
                case "2": return "Regime de apuração dos tributos federais pelo SN e ISSQN por fora do SN";
                case "3": return "Regime de apuração dos tributos federais e municipal por fora do SN";
                default: return Vazio;
            }
        }

        public static string TributacaoIssqn(string tribISSQN)
        {
            switch (tribISSQN)
            {
                case "1": return "Operação Tributável";
                case "2": return "Exportação de Serviço";
                case "3": return "Não Incidência";
                case "4": return "Imunidade";
                default: return Vazio;
            }
        }

        public static string RetencaoIssqn(string tpRetISSQN)
        {
            switch (tpRetISSQN)
            {
                case "1": return "Não Retido";
                case "2": return "Retido pelo Tomador";
                case "3": return "Retido pelo Intermediário";
                default: return Vazio;
            }
        }

        public static string IndicadorOperacaoIbsCbs(string ind)
        {
            switch (ind)
            {
                case "1": return "Operação Interna";
                case "2": return "Importação";
                case "3": return "Exportação";
                default: return Vazio;
            }
        }

        /// <summary>
        /// "Situação da NFS-e": cancelada / substituída / MEI / normal.
        /// </summary>
        public static string SituacaoNfse(NfseDocumento n)
        {
            if (n.Cancelada) return "NFS-e Cancelada";
            if (!string.IsNullOrWhiteSpace(n.ChaveSubstituida)) return "NFS-e Substituída";
            if (n.Val != null && n.Val.OpSimpNac == "2") return "NFS-e MEI";
            return "NFS-e Normal";
        }

        /// <summary>
        /// Linha dos totais aproximados de tributos (Lei 12.741/2012).
        /// </summary>
        public static string TotaisAproximados(NfseDocumento.Valores v)
        {
            string fed = Vazio, est = Vazio, mun = Vazio;
            if (v != null)
            {
                if (v.IndTotTrib == "1")
                {
                    fed = Moeda(v.VTotTribFed);
                    est = Moeda(v.VTotTribEst);
                    mun = Moeda(v.VTotTribMun);
                }
                else if (v.IndTotTrib == "2")
                {
                    fed = Percentual(v.PTotTribFed);
                    est = Percentual(v.PTotTribEst);
                    mun = Percentual(v.PTotTribMun);
                }
            }
            return string.Format(
                "Totais aproximados dos Tributos cfe. Lei n° 12.741/2012: Federais: {0}; Estaduais: {1}; Municipais: {2};",
                fed, est, mun);
        }
    }
}
