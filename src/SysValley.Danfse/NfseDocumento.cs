using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace SysValley.Danfse
{
    /// <summary>
    /// Modelo de leitura do XML da NFS-e Nacional (layout SEFIN Nacional, namespace
    /// http://www.sped.fazenda.gov.br/nfse). Guarda os valores "crus" do XML; a
    /// formatacao para exibicao fica por conta de <see cref="DanfseFormato"/>.
    /// </summary>
    public sealed class NfseDocumento
    {
        // ----- infNFSe -------------------------------------------------------
        public string Versao { get; set; }
        public string ChaveAcesso { get; set; }
        public string XLocEmi { get; set; }
        public string XLocPrestacao { get; set; }
        public string NNFSe { get; set; }
        public string CLocIncid { get; set; }
        public string XLocIncid { get; set; }
        public string XTribNac { get; set; }
        public string XTribMun { get; set; }
        public string XNBS { get; set; }
        public string VerAplic { get; set; }
        public string AmbGer { get; set; }
        public string TpEmis { get; set; }
        public string ProcEmi { get; set; }
        public string CStat { get; set; }
        public DateTime? DhProc { get; set; }
        public string NDFSe { get; set; }
        public decimal? VLiq { get; set; }
        public decimal? VCalcDR { get; set; }
        public decimal? VCalcBM { get; set; }
        public decimal? VTotalRet { get; set; }

        /// <summary>Preenchido quando a NFS-e possui evento/marca de cancelamento.</summary>
        public bool Cancelada { get; set; }
        /// <summary>Chave da NFS-e substituida (grupo subst), quando houver.</summary>
        public string ChaveSubstituida { get; set; }

        public Emitente Emit { get; set; }

        // ----- infDPS --------------------------------------------------------
        public string TpAmb { get; set; }
        public DateTime? DhEmi { get; set; }
        public string SerieDps { get; set; }
        public string NDps { get; set; }
        public DateTime? DCompet { get; set; }
        public string TpEmit { get; set; }
        public string CLocEmi { get; set; }

        public Pessoa Prest { get; set; }
        public Pessoa Toma { get; set; }
        public Pessoa Interm { get; set; }
        public Pessoa Dest { get; set; }

        public Servico Serv { get; set; }
        public Valores Val { get; set; }

        // ---------------------------------------------------------------------
        public sealed class Endereco
        {
            public string XLgr { get; set; }
            public string Nro { get; set; }
            public string XCpl { get; set; }
            public string XBairro { get; set; }
            public string CMun { get; set; }
            public string XMun { get; set; }
            public string Uf { get; set; }
            public string Cep { get; set; }
            public string CPais { get; set; }
            public string XPais { get; set; }

            /// <summary>"BLUMENAU, 923, DOS MUNICIPIOS" (partes vazias sao omitidas).</summary>
            public string Linha
            {
                get
                {
                    var partes = new[] { XLgr, Nro, XCpl, XBairro }
                        .Where(p => !string.IsNullOrWhiteSpace(p));
                    return string.Join(", ", partes);
                }
            }
        }

        public class Pessoa
        {
            public string Cnpj { get; set; }
            public string Cpf { get; set; }
            public string Nif { get; set; }
            public string CaepF { get; set; }
            public string Im { get; set; }
            public string Ie { get; set; }
            public string XNome { get; set; }
            public string Fone { get; set; }
            public string Email { get; set; }
            public Endereco End { get; set; }

            /// <summary>CNPJ, senao CPF, senao NIF.</summary>
            public string Documento
            {
                get
                {
                    if (!string.IsNullOrWhiteSpace(Cnpj)) return Cnpj;
                    if (!string.IsNullOrWhiteSpace(Cpf)) return Cpf;
                    return Nif;
                }
            }
        }

        public sealed class Emitente : Pessoa
        {
            public string XFant { get; set; }
        }

        public sealed class Servico
        {
            public string CLocPrestacao { get; set; }
            public string CPaisPrestacao { get; set; }
            public string CTribNac { get; set; }
            public string CTribMun { get; set; }
            public string CNbs { get; set; }
            public string XDescServ { get; set; }
            public string XInfComp { get; set; }
        }

        public sealed class Valores
        {
            public decimal? VServ { get; set; }
            public decimal? VDescIncond { get; set; }
            public decimal? VDescCond { get; set; }
            public decimal? VDedRed { get; set; }

            // --- ISSQN
            public string TribIssqn { get; set; }
            public string TpRetIssqn { get; set; }
            public string TpImunidade { get; set; }
            public string CPaisResult { get; set; }
            public decimal? PAliq { get; set; }
            public decimal? VBc { get; set; }
            public decimal? VIssqn { get; set; }

            // --- Federais
            public decimal? VRetIrrf { get; set; }
            public decimal? VRetCp { get; set; }
            public decimal? VRetCsll { get; set; }
            public decimal? VPis { get; set; }
            public decimal? VCofins { get; set; }
            public string XDescCsll { get; set; }

            // --- Simples Nacional (grupo prest/regTrib)
            public string OpSimpNac { get; set; }
            public string RegApTribSn { get; set; }
            public string RegEspTrib { get; set; }

            // --- IBS/CBS (reforma tributaria)
            public string Cst { get; set; }
            public string CClassTrib { get; set; }
            public string IndOperacao { get; set; }
            public string CMunIncidIbs { get; set; }
            public string XMunIncidIbs { get; set; }
            public string UfIncidIbs { get; set; }
            public decimal? VExclusoesRedBc { get; set; }
            public decimal? VBcIbsCbs { get; set; }
            public decimal? PRedAliqIbs { get; set; }
            public decimal? PRedAliqCbs { get; set; }
            public decimal? PAliqIbsUf { get; set; }
            public decimal? PAliqIbsMun { get; set; }
            public decimal? PAliqEfetIbsMun { get; set; }
            public decimal? VIbsMun { get; set; }
            public decimal? PAliqEfetIbsUf { get; set; }
            public decimal? VIbsUf { get; set; }
            public decimal? VIbs { get; set; }
            public decimal? PAliqCbs { get; set; }
            public decimal? PAliqEfetCbs { get; set; }
            public decimal? VCbs { get; set; }

            // --- Totais aproximados (Lei 12.741/2012)
            public string IndTotTrib { get; set; }
            public decimal? VTotTribFed { get; set; }
            public decimal? VTotTribEst { get; set; }
            public decimal? VTotTribMun { get; set; }
            public decimal? PTotTribFed { get; set; }
            public decimal? PTotTribEst { get; set; }
            public decimal? PTotTribMun { get; set; }

            /// <summary>Soma do IBS + CBS apurados.</summary>
            public decimal TotalIbsCbs
            {
                get { return (VIbs ?? 0m) + (VCbs ?? 0m); }
            }

            /// <summary>True quando o XML traz o grupo IBS/CBS da reforma tributaria.</summary>
            public bool PossuiIbsCbs { get; set; }
        }

        // =====================================================================
        //  Carga
        // =====================================================================

        public static NfseDocumento CarregarArquivo(string caminhoXml)
        {
            if (string.IsNullOrWhiteSpace(caminhoXml)) throw new ArgumentNullException("caminhoXml");
            if (!File.Exists(caminhoXml)) throw new FileNotFoundException("XML da NFS-e nao encontrado.", caminhoXml);

            // Le em bytes para que a codificacao declarada no prologo seja respeitada.
            return Carregar(File.ReadAllBytes(caminhoXml));
        }

        /// <summary>Le a NFS-e a partir da string do XML.</summary>
        public static NfseDocumento Carregar(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml)) throw new ArgumentNullException("xml");

            // BOM ou espacos antes do prologo fazem XDocument.Parse falhar com
            // "Data at the root level is invalid" - situacao comum quando o XML vem
            // de webservice ou de uma leitura de arquivo com BOM.
            string conteudo = xml.TrimStart('\uFEFF', '\u200B', ' ', '\t', '\r', '\n');
            return Carregar(XDocument.Parse(conteudo));
        }

        /// <summary>
        /// Le a NFS-e a partir do XML em bytes, respeitando a codificacao declarada
        /// no prologo do documento (e o BOM, quando presente).
        /// </summary>
        public static NfseDocumento Carregar(byte[] xml)
        {
            if (xml == null || xml.Length == 0) throw new ArgumentNullException("xml");
            using (var ms = new MemoryStream(xml))
                return Carregar(XDocument.Load(ms));
        }

        public static NfseDocumento Carregar(XDocument doc)
        {
            if (doc == null) throw new ArgumentNullException("doc");

            // Aceita tanto <NFSe> quanto o XML ja "desembrulhado" em <infNFSe>.
            XElement raiz = doc.Root;
            XElement infNfse = raiz.Name.LocalName == "infNFSe" ? raiz : Filho(raiz, "infNFSe");
            if (infNfse == null)
                throw new InvalidOperationException("XML invalido: elemento <infNFSe> nao encontrado.");

            var n = new NfseDocumento();
            n.Versao = Attr(raiz, "versao") ?? Attr(infNfse, "versao");

            // Id = "NFS" + chave de 50 digitos
            string id = Attr(infNfse, "Id") ?? string.Empty;
            n.ChaveAcesso = id.StartsWith("NFS", StringComparison.OrdinalIgnoreCase) ? id.Substring(3) : id;

            n.XLocEmi = Texto(infNfse, "xLocEmi");
            n.XLocPrestacao = Texto(infNfse, "xLocPrestacao");
            n.NNFSe = Texto(infNfse, "nNFSe");
            n.CLocIncid = Texto(infNfse, "cLocIncid");
            n.XLocIncid = Texto(infNfse, "xLocIncid");
            n.XTribNac = Texto(infNfse, "xTribNac");
            n.XTribMun = Texto(infNfse, "xTribMun");
            n.XNBS = Texto(infNfse, "xNBS");
            n.VerAplic = Texto(infNfse, "verAplic");
            n.AmbGer = Texto(infNfse, "ambGer");
            n.TpEmis = Texto(infNfse, "tpEmis");
            n.ProcEmi = Texto(infNfse, "procEmi");
            n.CStat = Texto(infNfse, "cStat");
            n.DhProc = DataHora(Texto(infNfse, "dhProc"));
            n.NDFSe = Texto(infNfse, "nDFSe");

            XElement valNfse = Filho(infNfse, "valores");
            if (valNfse != null)
            {
                n.VLiq = Dec(Texto(valNfse, "vLiq"));
                n.VCalcDR = Dec(Texto(valNfse, "vCalcDR"));
                n.VCalcBM = Dec(Texto(valNfse, "vCalcBM"));
                n.VTotalRet = Dec(Texto(valNfse, "vTotalRet"));
            }

            XElement subst = Filho(infNfse, "subst");
            if (subst != null) n.ChaveSubstituida = Texto(subst, "chSubstda");
            n.Cancelada = Filho(infNfse, "NFSeCancelamento") != null
                          || string.Equals(Texto(infNfse, "situacao"), "cancelada", StringComparison.OrdinalIgnoreCase);

            // ----- emit
            XElement emit = Filho(infNfse, "emit");
            if (emit != null)
            {
                var e = new Emitente
                {
                    Cnpj = Texto(emit, "CNPJ"),
                    Cpf = Texto(emit, "CPF"),
                    Nif = Texto(emit, "NIF"),
                    Im = Texto(emit, "IM"),
                    XNome = Texto(emit, "xNome"),
                    XFant = Texto(emit, "xFant"),
                    Fone = Texto(emit, "fone"),
                    Email = Texto(emit, "email"),
                    End = LerEndereco(Filho(emit, "enderNac") ?? Filho(emit, "end"))
                };
                n.Emit = e;
            }

            // ----- DPS
            XElement dps = Filho(infNfse, "DPS");
            XElement infDps = dps != null ? Filho(dps, "infDPS") : null;
            if (infDps != null) LerInfDps(n, infDps);

            n.Val = n.Val ?? new Valores();
            n.Serv = n.Serv ?? new Servico();

            // Municipio de incidencia do IBS, quando ausente, segue o de incidencia do ISSQN.
            if (string.IsNullOrEmpty(n.Val.CMunIncidIbs)) n.Val.XMunIncidIbs = null;

            return n;
        }

        private static void LerInfDps(NfseDocumento n, XElement infDps)
        {
            n.TpAmb = Texto(infDps, "tpAmb");
            n.DhEmi = DataHora(Texto(infDps, "dhEmi"));
            n.SerieDps = Texto(infDps, "serie");
            n.NDps = Texto(infDps, "nDPS");
            n.DCompet = DataHora(Texto(infDps, "dCompet"));
            n.TpEmit = Texto(infDps, "tpEmit");
            n.CLocEmi = Texto(infDps, "cLocEmi");

            n.Prest = LerPessoa(Filho(infDps, "prest"));
            n.Toma = LerPessoa(Filho(infDps, "toma"));
            n.Interm = LerPessoa(Filho(infDps, "interm"));
            n.Dest = LerPessoa(Filho(infDps, "dest"));

            // ----- serv
            var serv = new Servico();
            XElement xServ = Filho(infDps, "serv");
            if (xServ != null)
            {
                XElement locPrest = Filho(xServ, "locPrest");
                if (locPrest != null)
                {
                    serv.CLocPrestacao = Texto(locPrest, "cLocPrestacao");
                    serv.CPaisPrestacao = Texto(locPrest, "cPaisPrestacao");
                }
                XElement cServ = Filho(xServ, "cServ");
                if (cServ != null)
                {
                    serv.CTribNac = Texto(cServ, "cTribNac");
                    serv.CTribMun = Texto(cServ, "cTribMun");
                    serv.CNbs = Texto(cServ, "cNBS");
                    serv.XDescServ = Texto(cServ, "xDescServ");
                }
                serv.XInfComp = Texto(xServ, "infoCompl", "xInfComp");
            }
            n.Serv = serv;

            // ----- valores
            var v = new Valores();
            XElement vals = Filho(infDps, "valores");
            if (vals != null)
            {
                XElement vsp = Filho(vals, "vServPrest");
                if (vsp != null)
                {
                    v.VServ = Dec(Texto(vsp, "vServ"));
                    v.VDescIncond = Dec(Texto(vsp, "vDescIncond"));
                    v.VDescCond = Dec(Texto(vsp, "vDescCond"));
                }
                XElement vdr = Filho(vals, "vDescCondIncond");
                if (vdr != null)
                {
                    v.VDescIncond = v.VDescIncond ?? Dec(Texto(vdr, "vDescIncond"));
                    v.VDescCond = v.VDescCond ?? Dec(Texto(vdr, "vDescCond"));
                }
                XElement ded = Filho(vals, "vDedRed");
                if (ded != null) v.VDedRed = Dec(Texto(ded, "vDR")) ?? Dec(Texto(ded, "pDR"));

                XElement trib = Filho(vals, "trib");
                if (trib != null) LerTributos(v, trib);
            }

            // Simples Nacional fica no grupo prest/regTrib
            XElement prest = Filho(infDps, "prest");
            XElement regTrib = prest != null ? Filho(prest, "regTrib") : null;
            if (regTrib != null)
            {
                v.OpSimpNac = Texto(regTrib, "opSimpNac");
                v.RegApTribSn = Texto(regTrib, "regApTribSN");
                v.RegEspTrib = Texto(regTrib, "regEspTrib");
            }

            n.Val = v;
        }

        private static void LerTributos(Valores v, XElement trib)
        {
            XElement mun = Filho(trib, "tribMun");
            if (mun != null)
            {
                v.TribIssqn = Texto(mun, "tribISSQN");
                v.TpRetIssqn = Texto(mun, "tpRetISSQN");
                v.TpImunidade = Texto(mun, "tpImunidade");
                v.CPaisResult = Texto(mun, "cPaisResult");
                v.PAliq = Dec(Texto(mun, "pAliq"));
                v.VBc = Dec(Texto(mun, "vBC"));
                v.VIssqn = Dec(Texto(mun, "vISSQN"));
            }

            XElement fed = Filho(trib, "tribFed");
            if (fed != null)
            {
                XElement pc = Filho(fed, "piscofins");
                if (pc != null)
                {
                    v.VPis = Dec(Texto(pc, "vPis"));
                    v.VCofins = Dec(Texto(pc, "vCofins"));
                }
                v.VRetIrrf = Dec(Texto(fed, "vRetIRRF"));
                v.VRetCp = Dec(Texto(fed, "vRetCP"));
                v.VRetCsll = Dec(Texto(fed, "vRetCSLL"));
                v.XDescCsll = Texto(fed, "xDescCSLL");
            }

            XElement tot = Filho(trib, "totTrib");
            if (tot != null)
            {
                v.IndTotTrib = Texto(tot, "indTotTrib");
                XElement vt = Filho(tot, "vTotTrib");
                if (vt != null)
                {
                    v.VTotTribFed = Dec(Texto(vt, "vTotTribFed"));
                    v.VTotTribEst = Dec(Texto(vt, "vTotTribEst"));
                    v.VTotTribMun = Dec(Texto(vt, "vTotTribMun"));
                }
                XElement pt = Filho(tot, "pTotTrib");
                if (pt != null)
                {
                    v.PTotTribFed = Dec(Texto(pt, "pTotTribFed"));
                    v.PTotTribEst = Dec(Texto(pt, "pTotTribEst"));
                    v.PTotTribMun = Dec(Texto(pt, "pTotTribMun"));
                }
            }

            // Grupo IBS/CBS (reforma tributaria) - ainda opcional em 2026.
            XElement ibs = Filho(trib, "IBSCBS") ?? Filho(trib, "ibscbs");
            if (ibs != null)
            {
                v.PossuiIbsCbs = true;
                v.Cst = Texto(ibs, "CST");
                v.CClassTrib = Texto(ibs, "cClassTrib");
                v.IndOperacao = Texto(ibs, "indOperacao");
                v.CMunIncidIbs = Texto(ibs, "cMunIncid");
                v.UfIncidIbs = Texto(ibs, "UF");
                v.VExclusoesRedBc = Dec(Texto(ibs, "vExclusoes"));
                v.VBcIbsCbs = Dec(Texto(ibs, "vBC"));

                XElement gIbs = Filho(ibs, "gIBS") ?? ibs;
                v.PRedAliqIbs = Dec(Texto(gIbs, "pRedAliqIBS"));
                v.PAliqIbsUf = Dec(Texto(gIbs, "pAliqIBSUF"));
                v.PAliqIbsMun = Dec(Texto(gIbs, "pAliqIBSMun"));
                v.PAliqEfetIbsUf = Dec(Texto(gIbs, "pAliqEfetIBSUF"));
                v.PAliqEfetIbsMun = Dec(Texto(gIbs, "pAliqEfetIBSMun"));
                v.VIbsUf = Dec(Texto(gIbs, "vIBSUF"));
                v.VIbsMun = Dec(Texto(gIbs, "vIBSMun"));
                v.VIbs = Dec(Texto(gIbs, "vIBS"));

                XElement gCbs = Filho(ibs, "gCBS") ?? ibs;
                v.PRedAliqCbs = Dec(Texto(gCbs, "pRedAliqCBS"));
                v.PAliqCbs = Dec(Texto(gCbs, "pAliqCBS"));
                v.PAliqEfetCbs = Dec(Texto(gCbs, "pAliqEfetCBS"));
                v.VCbs = Dec(Texto(gCbs, "vCBS"));
            }
        }

        private static Pessoa LerPessoa(XElement el)
        {
            if (el == null) return null;
            var p = new Pessoa
            {
                Cnpj = Texto(el, "CNPJ"),
                Cpf = Texto(el, "CPF"),
                Nif = Texto(el, "NIF"),
                CaepF = Texto(el, "CAEPF"),
                Im = Texto(el, "IM"),
                Ie = Texto(el, "IE"),
                XNome = Texto(el, "xNome"),
                Fone = Texto(el, "fone"),
                Email = Texto(el, "email")
            };

            XElement end = Filho(el, "end") ?? Filho(el, "enderNac");
            if (end != null)
            {
                // <end> pode conter <endNac> (nacional) ou <endExt> (exterior)
                XElement nac = Filho(end, "endNac");
                XElement ext = Filho(end, "endExt");
                p.End = LerEndereco(end);
                if (nac != null)
                {
                    p.End.CMun = Texto(nac, "cMun");
                    p.End.Cep = Texto(nac, "CEP");
                }
                else if (ext != null)
                {
                    p.End.CPais = Texto(ext, "cPais");
                    p.End.XPais = Texto(ext, "xPais");
                    p.End.Cep = Texto(ext, "cEndPost");
                    p.End.XMun = Texto(ext, "xCidade");
                    p.End.Uf = Texto(ext, "xEstProvReg");
                }
            }
            return p;
        }

        private static Endereco LerEndereco(XElement el)
        {
            if (el == null) return null;
            return new Endereco
            {
                XLgr = Texto(el, "xLgr"),
                Nro = Texto(el, "nro"),
                XCpl = Texto(el, "xCpl"),
                XBairro = Texto(el, "xBairro"),
                CMun = Texto(el, "cMun"),
                XMun = Texto(el, "xMun"),
                Uf = Texto(el, "UF"),
                Cep = Texto(el, "CEP"),
                CPais = Texto(el, "cPais"),
                XPais = Texto(el, "xPais")
            };
        }

        // =====================================================================
        //  Utilitarios de navegacao (independentes de namespace)
        // =====================================================================

        private static XElement Filho(XElement pai, string nomeLocal)
        {
            if (pai == null) return null;
            return pai.Elements().FirstOrDefault(
                e => string.Equals(e.Name.LocalName, nomeLocal, StringComparison.Ordinal));
        }

        private static string Texto(XElement pai, params string[] caminho)
        {
            XElement atual = pai;
            for (int i = 0; i < caminho.Length && atual != null; i++)
                atual = Filho(atual, caminho[i]);
            if (atual == null) return null;
            string s = atual.Value;
            return string.IsNullOrWhiteSpace(s) ? null : s.Trim();
        }

        private static string Attr(XElement el, string nome)
        {
            if (el == null) return null;
            XAttribute a = el.Attributes().FirstOrDefault(
                x => string.Equals(x.Name.LocalName, nome, StringComparison.Ordinal));
            return a == null ? null : a.Value;
        }

        private static decimal? Dec(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            decimal d;
            if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out d)) return d;
            if (decimal.TryParse(s, NumberStyles.Any, new CultureInfo("pt-BR"), out d)) return d;
            return null;
        }

        private static DateTime? DataHora(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            DateTime dt;
            if (DateTime.TryParse(s, CultureInfo.InvariantCulture,
                                  DateTimeStyles.None, out dt)) return dt;
            return null;
        }

    }
}
