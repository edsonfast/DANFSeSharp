using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using QRCoder;

namespace SysValley.Danfse
{
    /// <summary>
    /// Gera o DANFSe v2.0 (Documento Auxiliar da NFS-e Nacional) em PDF a partir do
    /// XML da NFS-e, reproduzindo o layout do ambiente nacional (SEFIN Nacional).
    ///
    /// Uso tipico:
    /// <code>
    /// DanfseGenerator.GerarArquivo(@"C:\nfse\nota.xml", @"C:\nfse\nota.pdf");
    /// </code>
    ///
    /// Dependencias (NuGet): PDFsharp 1.50.5147 e QRCoder 1.4.3.
    /// </summary>
    public sealed class DanfseGenerator
    {
        // =====================================================================
        //  Constantes de layout (todas em pontos - o PDF de referencia usa A4
        //  595 x 842 pt). Os valores foram extraidos do DANFSe oficial.
        // =====================================================================

        private const double PaginaLargura = 595.0;
        private const double PaginaAltura = 842.0;

        // Moldura externa
        private const double MolduraEsq = 5.0;
        private const double MolduraTopo = 5.0;
        private const double MolduraDir = 590.0;
        private const double MolduraBase = 837.0;

        // Area util / grade de 4 colunas
        private const double X0 = 8.5039;
        private const double X1 = 586.7716;
        private const double LargCol = (X1 - X0) / 4.0;      // 144.5669
        private const double PadX = 3.4016;                  // recuo do texto na celula

        // Metricas das fontes usadas (fracao do corpo): ascendente e altura de linha.
        private const double AscArial = 0.905, LinhaArial = 1.117;
        private const double AscMss = 0.9219, LinhaMss = 1.1319;

        private const double TamTitulo = 7.0;   // titulos de secao e rotulos do bloco de identificacao
        private const double TamRotulo = 6.0;   // rotulos comuns
        private const double TamValor = 7.0;    // valores

        private const double AlturaValor = TamValor * LinhaMss;   // 7.9233
        private const double PadBaixo = 4.249;                    // respiro inferior da linha

        // Cabecalho
        private const double CabTopo = 5.6693;
        private const double CabBase = 39.685;
        private const double YSeparadorCab = 39.93;

        /// <summary>Y da primeira linha de conteudo, logo abaixo do cabecalho.</summary>
        private const double YTopoConteudo = 44.4413;

        /// <summary>Y maximo do conteudo antes de quebrar a pagina.</summary>
        private const double YLimiteConteudo = 780.0;

        // QR Code (pagina 1)
        private const double QrX = 492.0, QrY = 44.8, QrLado = 45.0;
        private const double YTextoQr = 91.88;

        // Rodape
        private const double RodapeTopo = 795.80;
        private const double RodapeBase = 815.88;
        private const double PadRodape = 4.4;

        private static readonly XColor Cinza = XColor.FromArgb(242, 242, 242);

        // =====================================================================
        //  Configuracao publica
        // =====================================================================

        /// <summary>
        /// Tabela codigo IBGE (7 digitos) -> nome do municipio, usada quando o XML
        /// nao traz o nome (ex.: municipio do tomador). Os municipios citados no
        /// proprio XML sao resolvidos automaticamente.
        /// </summary>
        public IDictionary<string, string> MunicipiosIbge { get; set; }

        /// <summary>Modelo da URL gravada no QR Code. {0} = chave de acesso.</summary>
        public string UrlQrCode { get; set; }

        /// <summary>Logomarca do cabecalho (PNG/JPG). Nulo usa a logo NFS-e embutida.</summary>
        public byte[] Logotipo { get; set; }

        /// <summary>Texto explicativo ao lado do QR Code.</summary>
        public string TextoQrCode { get; set; }

        public DanfseGenerator()
        {
            UrlQrCode = "https://www.nfse.gov.br/ConsultaPublica?tpc=1&chave={0}";
            TextoQrCode = "A autenticidade desta NFS-e pode ser verificada pela leitura deste " +
                          "código QR ou pela consulta da chave de acesso no portal nacional da NFS-e";
        }

        // =====================================================================
        //  API estatica de conveniencia
        // =====================================================================

        /// <summary>Le o XML do disco e grava o PDF do DANFSe.</summary>
        public static void GerarArquivo(string caminhoXml, string caminhoPdf)
        {
            if (string.IsNullOrWhiteSpace(caminhoPdf)) throw new ArgumentNullException("caminhoPdf");
            byte[] pdf = new DanfseGenerator().Gerar(NfseDocumento.CarregarArquivo(caminhoXml));
            string dir = Path.GetDirectoryName(Path.GetFullPath(caminhoPdf));
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
            File.WriteAllBytes(caminhoPdf, pdf);
        }

        /// <summary>
        /// Atalho: recebe a string do XML da NFS-e e devolve os bytes do PDF, cabendo
        /// a gravacao (ou o envio por e-mail, HTTP, banco...) a aplicacao chamadora.
        /// Use a sobrecarga de instancia <see cref="Gerar(string)"/> quando precisar
        /// configurar tabela IBGE, URL do QR Code ou logomarca.
        /// </summary>
        public static byte[] GerarDeXml(string xml)
        {
            return new DanfseGenerator().Gerar(xml);
        }

        /// <summary>
        /// Atalho: recebe o XML da NFS-e ja em bytes (como costuma chegar de um
        /// webservice ou de um campo binario) e devolve os bytes do PDF. A codificacao
        /// declarada no proprio XML e respeitada.
        /// </summary>
        public static byte[] GerarDeXml(byte[] xml)
        {
            return new DanfseGenerator().Gerar(xml);
        }

        // =====================================================================
        //  Estado da renderizacao
        // =====================================================================

        private PdfDocument _pdf;
        private PdfPage _pagina;
        private XGraphics _gfx;
        private double _y;
        private int _numeroPagina;

        private XFont _fTitulo, _fRotulo, _fValor, _fCabTitulo, _fCabMunicipio, _fCabMini;
        private XSolidBrush _brCinza;
        private XPen _penFina, _penGrossa;

        private NfseDocumento _n;
        private readonly Dictionary<string, string> _municipios =
            new Dictionary<string, string>(StringComparer.Ordinal);
        private readonly List<IDisposable> _descartaveis = new List<IDisposable>();

        // =====================================================================
        //  Geracao
        // =====================================================================

        /// <summary>
        /// Gera o PDF do DANFSe a partir da string do XML da NFS-e e devolve os bytes
        /// do arquivo, sem tocar em disco: a aplicacao chamadora decide o que fazer
        /// com o resultado (gravar, anexar em e-mail, devolver numa API, etc.).
        /// </summary>
        /// <param name="xml">Conteudo XML da NFS-e (elemento &lt;NFSe&gt; ou &lt;infNFSe&gt;).</param>
        /// <returns>Bytes do PDF do DANFSe.</returns>
        /// <example>
        /// <code>
        /// var gerador = new DanfseGenerator();
        /// byte[] pdf = gerador.Gerar(xmlDaNota);
        /// File.WriteAllBytes(@"C:\nfse\nota.pdf", pdf);
        /// </code>
        /// </example>
        public byte[] Gerar(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
                throw new ArgumentException("O conteudo XML da NFS-e nao foi informado.", "xml");

            return Gerar(NfseDocumento.Carregar(xml));
        }

        /// <summary>
        /// Gera o PDF do DANFSe a partir do XML da NFS-e em bytes, respeitando a
        /// codificacao declarada no proprio documento.
        /// </summary>
        public byte[] Gerar(byte[] xml)
        {
            if (xml == null || xml.Length == 0)
                throw new ArgumentException("O conteudo XML da NFS-e nao foi informado.", "xml");

            return Gerar(NfseDocumento.Carregar(xml));
        }

        /// <summary>Gera o PDF do DANFSe e devolve os bytes do arquivo.</summary>
        public byte[] Gerar(NfseDocumento nfse)
        {
            if (nfse == null) throw new ArgumentNullException("nfse");
            _n = nfse;
            _numeroPagina = 0;

            IndexarMunicipios();

            _pdf = new PdfDocument();
            _pdf.Info.Title = "DANFSe " + F.OuVazio(_n.ChaveAcesso);
            _pdf.Info.Subject = "Documento Auxiliar da NFS-e";
            _pdf.Info.Creator = "SysValley.Danfse";

            var opcoes = new XPdfFontOptions(PdfFontEncoding.Unicode);
            _fCabTitulo = new XFont("Arial", 9, XFontStyle.Bold, opcoes);
            _fTitulo = new XFont("Arial", TamTitulo, XFontStyle.Bold, opcoes);
            _fRotulo = new XFont("Arial", TamRotulo, XFontStyle.Bold, opcoes);
            _fValor = new XFont("Microsoft Sans Serif", TamValor, XFontStyle.Regular, opcoes);
            _fCabMunicipio = new XFont("Microsoft Sans Serif", 8, XFontStyle.Regular, opcoes);
            _fCabMini = new XFont("Microsoft Sans Serif", 6, XFontStyle.Regular, opcoes);

            _brCinza = new XSolidBrush(Cinza);
            _penFina = new XPen(XColors.Black, 0.5);
            _penGrossa = new XPen(XColors.Black, 1.0);

            try
            {
                NovaPagina();

                BlocoIdentificacao();
                BlocoPrestador();
                BlocoPessoa("TOMADOR / ADQUIRENTE", _n.Toma,
                            "TOMADOR / ADQUIRENTE DA OPERAÇÃO NÃO IDENTIFICADO NA NFS-e");
                BlocoPessoa("DESTINATÁRIO", _n.Dest,
                            "DESTINATÁRIO DA OPERAÇÃO NÃO IDENTIFICADO NA NFS-e");
                BlocoPessoa("INTERMEDIÁRIO", _n.Interm,
                            "INTERMEDIÁRIO DA OPERAÇÃO NÃO IDENTIFICADO NA NFS-e");
                BlocoServico();
                BlocoTributacaoMunicipal();
                BlocoTributacaoFederal();
                BlocoTributacaoIbsCbs();
                BlocoValorTotal();
                BlocoInformacoesComplementares();

                DesenharRodape();

                using (var ms = new MemoryStream())
                {
                    _pdf.Save(ms, false);
                    return ms.ToArray();
                }
            }
            finally
            {
                if (_gfx != null) { _gfx.Dispose(); _gfx = null; }
                if (_pdf != null) { _pdf.Dispose(); _pdf = null; }
                for (int i = _descartaveis.Count - 1; i >= 0; i--)
                {
                    try { _descartaveis[i].Dispose(); }
                    catch { /* recursos graficos - descarte best-effort */ }
                }
                _descartaveis.Clear();
            }
        }

        // atalho de leitura
        private static class F
        {
            public const string Vazio = DanfseFormato.Vazio;
            public static string OuVazio(string s) { return DanfseFormato.OuVazio(s); }
        }

        // =====================================================================
        //  Paginacao
        // =====================================================================

        private void NovaPagina()
        {
            if (_gfx != null)
            {
                DesenharRodape();
                _gfx.Dispose();
            }

            _pagina = _pdf.AddPage();
            _pagina.Width = PaginaLargura;
            _pagina.Height = PaginaAltura;
            _gfx = XGraphics.FromPdfPage(_pagina);
            _numeroPagina++;

            _gfx.DrawRectangle(_penGrossa, MolduraEsq, MolduraTopo,
                               MolduraDir - MolduraEsq, MolduraBase - MolduraTopo);

            DesenharCabecalho();
            _y = YTopoConteudo;
        }

        private void GarantirEspaco(double altura)
        {
            if (_y + altura > YLimiteConteudo) NovaPagina();
        }

        // =====================================================================
        //  Cabecalho / rodape
        // =====================================================================

        private void DesenharCabecalho()
        {
            double h = CabBase - CabTopo;
            _gfx.DrawRectangle(_brCinza, ColX(0), CabTopo, LargCol, h);
            _gfx.DrawRectangle(_brCinza, ColX(1), CabTopo, LargCol * 2, h);
            _gfx.DrawRectangle(_brCinza, ColX(3), CabTopo, LargCol, h);

            // Logomarca NFS-e
            byte[] logo = Logotipo ?? DanfseRecursos.LogoNfse;
            if (logo != null && logo.Length > 0)
            {
                XImage img = CarregarImagem(logo, Cinza);   // a faixa do cabecalho e cinza
                if (img != null) _gfx.DrawImage(img, 11.9, 10.3, 115.7, 22.9);
            }

            // Titulo centralizado nas colunas 2 e 3
            double centro = ColX(1) + LargCol;      // 297.6378
            DesenharCentralizado("DANFSe v2.0", _fCabTitulo, centro, 12.62 + 9 * AscArial);
            DesenharCentralizado("Documento Auxiliar da NFS-e", _fCabTitulo, centro, 22.97 + 9 * AscArial);

            // Bloco da direita
            double xd = ColX(3) + PadX;
            string uf = _n.Emit != null && _n.Emit.End != null ? _n.Emit.End.Uf : null;
            uf = uf ?? DanfseFormato.UfPorCodigoIbge(_n.CLocEmi);
            string municipio = F.OuVazio(_n.XLocEmi) + (string.IsNullOrEmpty(uf) ? "" : " - " + uf);

            Texto("Município: " + municipio, _fCabMunicipio, xd, 11.36 + 8 * AscMss);
            Texto("Ambiente Gerador: " + F.OuVazio(_n.AmbGer), _fCabMini, xd, 20.415 + 6 * AscMss);
            Texto("Tipo de Ambiente: " + F.OuVazio(_n.TpAmb), _fCabMini, xd, 27.206 + 6 * AscMss);

            _gfx.DrawLine(_penFina, X0, YSeparadorCab, X1, YSeparadorCab);

            if (_numeroPagina == 1) DesenharQrCode();
        }

        private void DesenharQrCode()
        {
            if (string.IsNullOrWhiteSpace(_n.ChaveAcesso)) return;

            string url = string.Format(UrlQrCode, _n.ChaveAcesso);
            byte[] png;
            using (var gerador = new QRCodeGenerator())
            using (QRCodeData dados = gerador.CreateQrCode(url, QRCodeGenerator.ECCLevel.M))
            {
                var qr = new PngByteQRCode(dados);
                // Sem zona de silencio: o espaco em branco ao redor no DANFSe ja cumpre esse papel.
                png = qr.GetGraphic(20, new byte[] { 0, 0, 0, 255 }, new byte[] { 255, 255, 255, 255 }, false);
            }

            XImage img = CarregarImagem(png, XColors.White);
            if (img != null) _gfx.DrawImage(img, QrX, QrY, QrLado, QrLado);

            double xd = ColX(3) + PadX;
            double larguraMax = LargCol - 2 * PadX;
            string[] linhas = Quebrar(TextoQrCode, _fCabMini, larguraMax);
            double alturaLinha = 6 * LinhaMss;
            for (int i = 0; i < linhas.Length; i++)
                Texto(linhas[i], _fCabMini, xd, YTextoQr + i * alturaLinha + 6 * AscMss);
        }

        private void DesenharRodape()
        {
            CaixaRodape(0, 1);
            CaixaRodape(1, 1);
            CaixaRodape(2, 2);

            double baseRotulo = RodapeTopo + 0.70 + TamRotulo * AscArial;
            double baseValor = RodapeTopo + 0.70 + TamRotulo * LinhaArial + TamValor * AscMss;

            Texto("DATA CIENTIFICAÇÃO:", _fRotulo, ColX(0) + PadRodape, baseRotulo);
            Texto("IDENTIFICAÇÃO E ASSINATURA", _fRotulo, ColX(1) + PadRodape, baseRotulo);
            Texto("N° NFS-e / CHAVE NFS-e", _fRotulo, ColX(2) + PadRodape, baseRotulo);
            Texto(F.OuVazio(_n.NNFSe) + " / " + F.OuVazio(_n.ChaveAcesso),
                  _fValor, ColX(2) + PadRodape, baseValor);
        }

        private void CaixaRodape(int coluna, int span)
        {
            double xi = ColX(coluna);
            double xf = ColX(coluna + span) + 1.0;
            _gfx.DrawLine(_penGrossa, xi, RodapeTopo, xf, RodapeTopo);
            _gfx.DrawLine(_penGrossa, xi, RodapeBase, xf, RodapeBase);
            _gfx.DrawLine(_penGrossa, xi + 0.5, RodapeTopo + 0.5, xi + 0.5, RodapeBase + 0.5);
            _gfx.DrawLine(_penGrossa, xf - 0.5, RodapeTopo + 0.5, xf - 0.5, RodapeBase + 0.5);
        }

        // =====================================================================
        //  Blocos do documento
        // =====================================================================

        private void BlocoIdentificacao()
        {
            // Rotulos deste bloco sao 7pt (linhas mais altas que o restante).
            Linha(TamTitulo,
                Campo(0, "CHAVE DE ACESSO DA NFS-e", F.OuVazio(_n.ChaveAcesso), 3));

            Linha(TamTitulo,
                Campo(0, "NÚMERO DA NFS-e", F.OuVazio(_n.NNFSe)),
                Campo(1, "COMPETÊNCIA DA NFS-e", DanfseFormato.Data(_n.DCompet)),
                Campo(2, "DATA E HORA DA EMISSÃO DA NFS-e", DanfseFormato.DataHora(_n.DhProc)));

            Linha(TamTitulo,
                Campo(0, "NÚMERO DA DPS", F.OuVazio(_n.NDps)),
                Campo(1, "SÉRIE DA DPS", F.OuVazio(_n.SerieDps)),
                Campo(2, "DATA E HORA DA EMISSÃO DA DPS", DanfseFormato.DataHora(_n.DhEmi)));

            Linha(TamTitulo,
                Cinzento(Campo(0, "EMITENTE DA NFS-e", DanfseFormato.TipoEmitente(_n.TpEmit))),
                Campo(1, "SITUAÇÃO DA NFS-e", DanfseFormato.SituacaoNfse(_n)),
                Campo(2, "FINALIDADE", Finalidade()));

            Separador();
        }

        private string Finalidade()
        {
            return string.IsNullOrWhiteSpace(_n.ChaveSubstituida) ? F.Vazio : "Substituição de NFS-e";
        }

        private void BlocoPrestador()
        {
            NfseDocumento.Pessoa p = _n.Prest;
            NfseDocumento.Emitente e = _n.Emit;

            string doc = (e != null ? e.Documento : null) ?? (p != null ? p.Documento : null);
            string im = (e != null ? e.Im : null) ?? (p != null ? p.Im : null);
            string fone = (p != null ? p.Fone : null) ?? (e != null ? e.Fone : null);
            string email = (p != null ? p.Email : null) ?? (e != null ? e.Email : null);
            NfseDocumento.Endereco end = e != null ? e.End : null;

            Linha(TamRotulo,
                Titulo(0, "PRESTADOR / FORNECEDOR"),
                Campo(1, "CNPJ / CPF / NIF", DanfseFormato.CnpjCpf(doc)),
                Campo(2, "Indicador Municipal (Inscrição)", F.OuVazio(im)),
                Campo(3, "Telefone", DanfseFormato.Telefone(fone)));

            Linha(TamRotulo,
                Campo(0, "Nome / Nome Empresarial", F.OuVazio(e != null ? e.XNome : null), 2),
                Campo(2, "Município / Sigla UF", MunicipioUf(end)),
                Campo(3, "Código IBGE / CEP", IbgeCep(end)));

            Linha(TamRotulo,
                Campo(0, "Endereço", F.OuVazio(end != null ? end.Linha : null), 2),
                Campo(2, "E-mail", F.OuVazio(email), 2));

            Linha(TamRotulo,
                Campo(0, "Simples Nacional na Data de Competência",
                         DanfseFormato.OpcaoSimplesNacional(_n.Val.OpSimpNac)),
                Campo(1, "Regime de Apuração Tributária pelo SN",
                         DanfseFormato.RegimeApuracaoSn(_n.Val.RegApTribSn), 3));

            Separador();
        }

        /// <summary>Bloco de 3 linhas do tomador/destinatario/intermediario, ou a faixa "nao identificado".</summary>
        private void BlocoPessoa(string titulo, NfseDocumento.Pessoa p, string textoAusente)
        {
            if (p == null)
            {
                Faixa(textoAusente);
                return;
            }

            Linha(TamRotulo,
                Titulo(0, titulo),
                Campo(1, "CNPJ / CPF / NIF", DanfseFormato.CnpjCpf(p.Documento)),
                Campo(2, "Indicador Municipal (Inscrição)", F.OuVazio(p.Im)),
                Campo(3, "Telefone", DanfseFormato.Telefone(p.Fone)));

            Linha(TamRotulo,
                Campo(0, "Nome / Nome Empresarial", F.OuVazio(p.XNome), 2),
                Campo(2, "Município / Sigla UF", MunicipioUf(p.End)),
                Campo(3, "Código IBGE / CEP", IbgeCep(p.End)));

            Linha(TamRotulo,
                Campo(0, "Endereço", F.OuVazio(p.End != null ? p.End.Linha : null), 2),
                Campo(2, "E-mail", F.OuVazio(p.Email), 2));

            Separador();
        }

        private void BlocoServico()
        {
            NfseDocumento.Servico s = _n.Serv;

            string local = DanfseFormato.Juntar(" / ",
                _n.XLocPrestacao ?? Municipio(s.CLocPrestacao),
                DanfseFormato.UfPorCodigoIbge(s.CLocPrestacao),
                null /* pais: nao informado no layout nacional para prestacao interna */);

            Linha(TamRotulo,
                Titulo(0, "SERVIÇO PRESTADO"),
                Campo(1, "Código de Tributação Nacional/Municipal",
                         DanfseFormato.Juntar(" / ",
                             DanfseFormato.CodigoTributacaoNacional(s.CTribNac), s.CTribMun)),
                Campo(2, "Código da NBS", DanfseFormato.Nbs(s.CNbs)),
                Campo(3, "Local da Prestação / Sigla UF / País", local));

            // Descricao oficial do codigo de tributacao (xTribNac / xTribMun)
            var descricoes = new List<string>();
            if (!string.IsNullOrWhiteSpace(_n.XTribNac)) descricoes.Add(_n.XTribNac);
            if (!string.IsNullOrWhiteSpace(_n.XTribMun)) descricoes.Add(_n.XTribMun);
            if (descricoes.Count > 0)
                Linha(0, TextoLivre(0, string.Join(" ", descricoes), 4));

            Linha(TamRotulo,
                TextoRotulado(0, "Descrição do Serviço", F.OuVazio(s.XDescServ), 4));

            Separador();
        }

        private void BlocoTributacaoMunicipal()
        {
            NfseDocumento.Valores v = _n.Val;

            string incidencia = DanfseFormato.Juntar(" / ",
                _n.XLocIncid ?? Municipio(_n.CLocIncid),
                DanfseFormato.UfPorCodigoIbge(_n.CLocIncid),
                null);

            Linha(TamRotulo,
                Titulo(0, "TRIBUTAÇÃO MUNICIPAL (ISSQN)"),
                Campo(1, "Tipo de Tributação do ISSQN", DanfseFormato.TributacaoIssqn(v.TribIssqn)),
                Campo(2, "Município / Sigla UF / País de Incidência do ISSQN", incidencia, 2));

            Linha(TamRotulo,
                Campo(0, "BC ISSQN", DanfseFormato.Moeda(v.VBc)),
                Campo(1, "Alíquota Aplicada", DanfseFormato.Percentual(v.PAliq)),
                Campo(2, "Retenção do ISSQN", DanfseFormato.RetencaoIssqn(v.TpRetIssqn)),
                Campo(3, "ISSQN Apurado", DanfseFormato.Moeda(v.VIssqn)));

            Separador();
        }

        private void BlocoTributacaoFederal()
        {
            NfseDocumento.Valores v = _n.Val;

            Linha(TamRotulo,
                Titulo(0, "TRIBUTAÇÃO FEDERAL (EXCETO CBS)"),
                Campo(1, "IRRF", DanfseFormato.Moeda(v.VRetIrrf)),
                Campo(2, "Contribuição Previdenciária - Retida", DanfseFormato.Moeda(v.VRetCp)),
                Campo(3, "Contribuições Sociais - Retidas", DanfseFormato.Moeda(v.VRetCsll)));

            Linha(TamRotulo,
                Campo(0, "PIS - Débito Apuração Própria", DanfseFormato.Moeda(v.VPis)),
                Campo(1, "COFINS - Débito Apuração Própria", DanfseFormato.Moeda(v.VCofins)),
                Campo(2, "Descrição Contrib. Sociais - Retidas", F.OuVazio(v.XDescCsll), 2));

            Separador();
        }

        private void BlocoTributacaoIbsCbs()
        {
            NfseDocumento.Valores v = _n.Val;

            string operacao = DanfseFormato.Juntar(" / ",
                DanfseFormato.IndicadorOperacaoIbsCbs(v.IndOperacao),
                DanfseFormato.Ibge(v.CMunIncidIbs),
                Municipio(v.CMunIncidIbs),
                v.UfIncidIbs ?? DanfseFormato.UfPorCodigoIbge(v.CMunIncidIbs));

            Linha(TamRotulo,
                Titulo(0, "TRIBUTAÇÃO IBS/CBS"),
                Campo(1, "CST / cClassTrib", DanfseFormato.Juntar(" / ", v.Cst, v.CClassTrib)),
                Campo(2, "Indicador de Operação / Código IBGE Incidência / Município Incidência / Sigla UF",
                         operacao, 2));

            // Observacao: o gerador oficial imprime tres componentes neste campo
            // ("- / - / -") embora o rotulo cite apenas IBS e CBS. Mantido igual ao portal.
            string reducoes = DanfseFormato.Juntar(" / ",
                DanfseFormato.Percentual(v.PRedAliqIbs), DanfseFormato.Percentual(v.PRedAliqCbs), null);

            Linha(TamRotulo,
                Campo(0, "Exclusões e Reduções da Base de Cálculo", DanfseFormato.MoedaZero(v.VExclusoesRedBc)),
                Campo(1, "Base de Cálculo Após Exclusões e Reduções", DanfseFormato.Moeda(v.VBcIbsCbs)),
                Campo(2, "Red. Alíquota IBS / Red. Alíquota CBS", reducoes),
                Campo(3, "Alíquota - IBS UF / IBS Mun",
                         DanfseFormato.Juntar(" / ",
                             DanfseFormato.Percentual(v.PAliqIbsUf), DanfseFormato.Percentual(v.PAliqIbsMun))));

            Linha(TamRotulo,
                Campo(0, "Alíq. Efetiva Municipal - IBS", DanfseFormato.Percentual(v.PAliqEfetIbsMun)),
                Campo(1, "Valor Apurado Municipal - IBS", DanfseFormato.Moeda(v.VIbsMun)),
                Campo(2, "Alíq. Efetiva Estadual - IBS", DanfseFormato.Percentual(v.PAliqEfetIbsUf)),
                Campo(3, "Valor Apurado Estadual - IBS", DanfseFormato.Moeda(v.VIbsUf)));

            Linha(TamRotulo,
                Campo(0, "Valor Total Apurado - IBS", DanfseFormato.Moeda(v.VIbs)),
                Campo(1, "Alíquota - CBS", DanfseFormato.Percentual(v.PAliqCbs)),
                Campo(2, "Alíquota Efetiva - CBS", DanfseFormato.Percentual(v.PAliqEfetCbs)),
                Campo(3, "Valor Total Apurado - CBS", DanfseFormato.Moeda(v.VCbs)));

            Separador();
        }

        private void BlocoValorTotal()
        {
            NfseDocumento.Valores v = _n.Val;

            // Enquanto o XML nao traz o grupo IBS/CBS (regra de transicao da reforma),
            // o portal nacional imprime R$ 0,00 nos dois campos de IBS/CBS.
            decimal? liquidoMaisIbsCbs = v.PossuiIbsCbs
                ? (_n.VLiq ?? 0m) + v.TotalIbsCbs
                : (decimal?)0m;

            Linha(TamRotulo,
                Titulo(0, "VALOR TOTAL DA NFS-e"),
                Campo(1, "VALOR DA OPERAÇÃO / SERVIÇO", DanfseFormato.Moeda(v.VServ)),
                Campo(2, "Desconto Incondicionado", DanfseFormato.Moeda(v.VDescIncond)),
                Campo(3, "Desconto Condicionado", DanfseFormato.Moeda(v.VDescCond)));

            Linha(TamRotulo,
                Campo(0, "Total das Retenções (ISSQN / Federais)", DanfseFormato.Moeda(_n.VTotalRet)),
                Campo(1, "VALOR LÍQUIDO DA NFS-e", DanfseFormato.Moeda(_n.VLiq)),
                Campo(2, "Total do IBS/CBS", DanfseFormato.MoedaZero(v.PossuiIbsCbs ? v.TotalIbsCbs : 0m)),
                Cinzento(Campo(3, "VALOR LÍQUIDO DA NFS-e + IBS/CBS", DanfseFormato.MoedaZero(liquidoMaisIbsCbs))));

            Separador();
        }

        private void BlocoInformacoesComplementares()
        {
            Linha(TamTitulo, Titulo(0, "INFORMAÇÕES COMPLEMENTARES", 4, false));

            var linhas = new List<string>();
            if (!string.IsNullOrWhiteSpace(_n.Serv.XInfComp))
                linhas.AddRange(Quebrar(_n.Serv.XInfComp, _fValor, 4 * LargCol - 2 * PadX));
            linhas.Add(string.Empty);
            linhas.Add(DanfseFormato.TotaisAproximados(_n.Val));

            Linha(0, new Cel
            {
                Col = 0,
                Span = 4,
                Valores = linhas.ToArray(),
                Truncar = false
            });
        }

        // =====================================================================
        //  Motor de desenho das linhas
        // =====================================================================

        private sealed class Cel
        {
            public int Col;
            public int Span = 1;
            public string Rotulo;
            public string[] Valores;
            public bool ApenasTitulo;   // rotulo em 7pt negrito, sem valor
            public bool Cinza;
            public bool Truncar = true;
        }

        private static Cel Campo(int col, string rotulo, string valor, int span = 1)
        {
            return new Cel { Col = col, Span = span, Rotulo = rotulo, Valores = new[] { valor } };
        }

        private static Cel Titulo(int col, string titulo, int span = 1, bool cinza = true)
        {
            return new Cel { Col = col, Span = span, Rotulo = titulo, ApenasTitulo = true, Cinza = cinza };
        }

        private static Cel Cinzento(Cel c) { c.Cinza = true; return c; }

        private Cel TextoLivre(int col, string texto, int span)
        {
            return new Cel
            {
                Col = col,
                Span = span,
                Valores = Quebrar(texto, _fValor, span * LargCol - 2 * PadX),
                Truncar = false
            };
        }

        private Cel TextoRotulado(int col, string rotulo, string texto, int span)
        {
            return new Cel
            {
                Col = col,
                Span = span,
                Rotulo = rotulo,
                Valores = Quebrar(texto, _fValor, span * LargCol - 2 * PadX),
                Truncar = false
            };
        }

        private static double ColX(int i) { return X0 + i * LargCol; }

        private static double AlturaRotulo(double tam)
        {
            return tam <= 0 ? 0 : tam / 30.0 + tam * LinhaArial;
        }

        private static double AlturaLinha(double tamRotulo, int qtdValores)
        {
            return AlturaRotulo(tamRotulo) + qtdValores * AlturaValor + PadBaixo;
        }

        /// <summary>
        /// Desenha uma linha da grade. Linhas com varias linhas de valor (descricao do
        /// servico, informacoes complementares) sao divididas entre paginas quando
        /// necessario: o rotulo aparece so na primeira parte.
        /// </summary>
        private void Linha(double tamRotulo, params Cel[] celulas)
        {
            int total = 0;
            foreach (Cel c in celulas)
                if (c.Valores != null && c.Valores.Length > total) total = c.Valores.Length;

            int inicio = 0;
            bool comRotulo = true;

            while (true)
            {
                double alturaRotulo = comRotulo ? AlturaRotulo(tamRotulo) : 0;
                int restantes = total - inicio;
                double h = alturaRotulo + restantes * AlturaValor + PadBaixo;

                if (_y + h <= YLimiteConteudo)
                {
                    Pintar(tamRotulo, comRotulo, celulas, inicio, restantes, h);
                    _y += h;
                    return;
                }

                // Quantas linhas de valor ainda cabem nesta pagina?
                int cabem = (int)Math.Floor(
                    (YLimiteConteudo - _y - alturaRotulo - PadBaixo) / AlturaValor);

                if (cabem < 1 || restantes <= 1)
                {
                    NovaPagina();   // nada util cabe aqui: recomeca na proxima pagina
                    continue;
                }

                double hParcial = alturaRotulo + cabem * AlturaValor + PadBaixo;
                Pintar(tamRotulo, comRotulo, celulas, inicio, cabem, hParcial);
                inicio += cabem;
                comRotulo = false;
                NovaPagina();
            }
        }

        /// <summary>Pinta uma linha (ou parte dela) a partir de <paramref name="inicio"/>.</summary>
        private void Pintar(double tamRotulo, bool comRotulo, Cel[] celulas,
                            int inicio, int quantidade, double altura)
        {
            foreach (Cel c in celulas)
                if (c.Cinza) _gfx.DrawRectangle(_brCinza, ColX(c.Col), _y, c.Span * LargCol, altura);

            double topoValores = _y + (comRotulo ? AlturaRotulo(tamRotulo) : 0);

            foreach (Cel c in celulas)
            {
                double x = ColX(c.Col) + PadX;
                double larguraMax = c.Span * LargCol - 2 * PadX;

                if (c.ApenasTitulo)
                {
                    if (comRotulo)
                        Texto(c.Rotulo, _fTitulo, x, _y + TamTitulo / 30.0 + TamTitulo * AscArial);
                    continue;
                }

                if (comRotulo && !string.IsNullOrEmpty(c.Rotulo))
                {
                    XFont fonte = tamRotulo >= TamTitulo ? _fTitulo : _fRotulo;
                    Texto(Cortar(c.Rotulo, fonte, larguraMax), fonte, x,
                          _y + tamRotulo / 30.0 + tamRotulo * AscArial);
                }

                if (c.Valores == null) continue;
                for (int i = 0; i < quantidade; i++)
                {
                    int idx = inicio + i;
                    if (idx >= c.Valores.Length) break;

                    string t = c.Valores[idx];
                    if (string.IsNullOrEmpty(t)) continue;
                    if (c.Truncar) t = Cortar(t, _fValor, larguraMax);
                    Texto(t, _fValor, x, topoValores + i * AlturaValor + TamValor * AscMss);
                }
            }
        }

        /// <summary>Linha unica com texto centralizado (blocos "nao identificado na NFS-e").</summary>
        private void Faixa(string texto)
        {
            GarantirEspaco(AlturaValor + 0.5);
            DesenharCentralizado(texto, _fValor, (X0 + X1) / 2.0, _y + TamValor * AscMss);
            _y += AlturaValor;
            Separador();
        }

        private void Separador()
        {
            _gfx.DrawLine(_penFina, X0, _y + 0.25, X1, _y + 0.25);
            _y += 0.5;
        }

        // =====================================================================
        //  Texto
        // =====================================================================

        private void Texto(string s, XFont fonte, double x, double baseline)
        {
            if (string.IsNullOrEmpty(s)) return;
            _gfx.DrawString(s, fonte, XBrushes.Black, x, baseline, XStringFormats.BaseLineLeft);
        }

        private void DesenharCentralizado(string s, XFont fonte, double centroX, double baseline)
        {
            if (string.IsNullOrEmpty(s)) return;
            double largura = _gfx.MeasureString(s, fonte).Width;
            Texto(s, fonte, centroX - largura / 2.0, baseline);
        }

        /// <summary>Corta o texto com reticencias quando ele nao cabe na largura da celula.</summary>
        private string Cortar(string texto, XFont fonte, double larguraMax)
        {
            if (string.IsNullOrEmpty(texto)) return texto;
            if (_gfx.MeasureString(texto, fonte).Width <= larguraMax) return texto;

            const string reticencias = "...";
            int n = texto.Length;
            while (n > 0 && _gfx.MeasureString(texto.Substring(0, n) + reticencias, fonte).Width > larguraMax)
                n--;
            return n <= 0 ? reticencias : texto.Substring(0, n) + reticencias;
        }

        /// <summary>Quebra o texto em linhas, respeitando as quebras existentes e a largura da celula.</summary>
        private string[] Quebrar(string texto, XFont fonte, double larguraMax)
        {
            if (string.IsNullOrEmpty(texto)) return new string[0];

            var saida = new List<string>();
            string[] paragrafos = texto.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');

            foreach (string paragrafo in paragrafos)
            {
                if (paragrafo.Length == 0) { saida.Add(string.Empty); continue; }
                if (_gfx.MeasureString(paragrafo, fonte).Width <= larguraMax) { saida.Add(paragrafo); continue; }

                var atual = new StringBuilder();
                foreach (string palavra in paragrafo.Split(' '))
                {
                    string teste = atual.Length == 0 ? palavra : atual + " " + palavra;
                    if (_gfx.MeasureString(teste, fonte).Width <= larguraMax)
                    {
                        atual.Clear();
                        atual.Append(teste);
                        continue;
                    }

                    if (atual.Length > 0) { saida.Add(atual.ToString()); atual.Clear(); }

                    // Palavra isolada maior que a celula: quebra por caractere.
                    string resto = palavra;
                    while (_gfx.MeasureString(resto, fonte).Width > larguraMax && resto.Length > 1)
                    {
                        int corte = resto.Length;
                        while (corte > 1 && _gfx.MeasureString(resto.Substring(0, corte), fonte).Width > larguraMax)
                            corte--;
                        saida.Add(resto.Substring(0, corte));
                        resto = resto.Substring(corte);
                    }
                    atual.Append(resto);
                }
                if (atual.Length > 0) saida.Add(atual.ToString());
            }

            return saida.ToArray();
        }

        // =====================================================================
        //  Municipios / enderecos
        // =====================================================================

        private void IndexarMunicipios()
        {
            _municipios.Clear();
            Indexar(_n.CLocEmi, _n.XLocEmi);
            Indexar(_n.CLocIncid, _n.XLocIncid);
            if (_n.Serv != null) Indexar(_n.Serv.CLocPrestacao, _n.XLocPrestacao);
            if (_n.Emit != null && _n.Emit.End != null)
            {
                Indexar(_n.Emit.End.CMun, _n.Emit.End.XMun);
                // O emitente esta sempre no municipio de emissao da DPS.
                Indexar(_n.Emit.End.CMun, _n.XLocEmi);
            }
            foreach (NfseDocumento.Pessoa p in new[] { _n.Prest, _n.Toma, _n.Interm, _n.Dest })
                if (p != null && p.End != null) Indexar(p.End.CMun, p.End.XMun);
        }

        private void Indexar(string cMun, string nome)
        {
            if (string.IsNullOrWhiteSpace(cMun) || string.IsNullOrWhiteSpace(nome)) return;
            if (!_municipios.ContainsKey(cMun)) _municipios[cMun] = nome.Trim();
        }

        private string Municipio(string cMun)
        {
            if (string.IsNullOrWhiteSpace(cMun)) return null;
            string nome;
            if (_municipios.TryGetValue(cMun, out nome)) return nome;
            if (MunicipiosIbge != null && MunicipiosIbge.TryGetValue(cMun, out nome)) return nome;
            return null;
        }

        private string MunicipioUf(NfseDocumento.Endereco end)
        {
            if (end == null) return F.Vazio;
            if (!string.IsNullOrWhiteSpace(end.XPais) && string.IsNullOrWhiteSpace(end.CMun))
                return DanfseFormato.Juntar(" / ", end.XMun, end.Uf);   // endereco no exterior

            return DanfseFormato.Juntar(" / ",
                Municipio(end.CMun) ?? end.XMun,
                end.Uf ?? DanfseFormato.UfPorCodigoIbge(end.CMun));
        }

        private string IbgeCep(NfseDocumento.Endereco end)
        {
            if (end == null) return F.Vazio;
            return DanfseFormato.Ibge(end.CMun) + " / " + DanfseFormato.Cep(end.Cep);
        }

        // =====================================================================
        //  Imagens
        // =====================================================================

        private XImage CarregarImagem(byte[] bytes, XColor fundo)
        {
            try
            {
                // O stream precisa continuar aberto ate o PdfDocument ser salvo:
                // por isso ele so e descartado no final da geracao.
                var ms = new MemoryStream(AchatarTransparencia(bytes, fundo));
                _descartaveis.Add(ms);
                XImage img = XImage.FromStream(ms);
                _descartaveis.Add(img);
                return img;
            }
            catch
            {
                return null;   // imagem invalida nao deve impedir a emissao do DANFSe
            }
        }

        /// <summary>
        /// O PDFsharp 1.50 grava o canal alfa de um PNG como preto. Para preservar a
        /// aparencia da logomarca, a transparencia e mesclada com a cor de fundo da faixa.
        /// </summary>
        private static byte[] AchatarTransparencia(byte[] bytes, XColor fundo)
        {
            using (var origem = new MemoryStream(bytes))
            using (var bmp = new System.Drawing.Bitmap(origem))
            {
                if (!System.Drawing.Image.IsAlphaPixelFormat(bmp.PixelFormat)) return bytes;

                using (var plano = new System.Drawing.Bitmap(
                           bmp.Width, bmp.Height,
                           System.Drawing.Imaging.PixelFormat.Format24bppRgb))
                using (var g = System.Drawing.Graphics.FromImage(plano))
                {
                    g.Clear(System.Drawing.Color.FromArgb(fundo.R, fundo.G, fundo.B));
                    g.DrawImage(bmp, 0, 0, bmp.Width, bmp.Height);
                    using (var saida = new MemoryStream())
                    {
                        plano.Save(saida, System.Drawing.Imaging.ImageFormat.Png);
                        return saida.ToArray();
                    }
                }
            }
        }
    }
}
