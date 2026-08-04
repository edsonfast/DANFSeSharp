using System;
using System.IO;
using SysValley.Danfse;

namespace Danfse.Exemplo
{
    internal static class Program
    {
        private static int Main(string[] args)
        {

            var _par = new string[1];

            _par[0] = @"nfse.xml";

            if (_par[0].Length < 1)
            {
                Console.WriteLine("Uso: Danfse.Exemplo <arquivo.xml> [saida.pdf]");
                return 1;
            }

            string xml = _par[0];
            string pdf = _par.Length > 1
                ? _par[1]
                : Path.ChangeExtension(xml, ".pdf");

            try
            {
                // --- Forma 1: string do XML -> bytes do PDF (a aplicacao grava) -------
                // E a forma indicada quando o XML ja esta em memoria (retorno do
                // webservice, coluna do banco, fila) e o destino do PDF nao e disco.
                string conteudoXml = File.ReadAllText(xml);

                var gerador = new DanfseGenerator();
                byte[] bytesPdf = gerador.Gerar(conteudoXml);

                File.WriteAllBytes(pdf, bytesPdf);   // ...ou anexar em e-mail, devolver numa API, gravar no banco

                // Atalho estatico equivalente, sem configuracao:
                //   byte[] bytesPdf = DanfseGenerator.GerarDeXml(conteudoXml);
                //
                // O XML tambem pode ser passado em bytes (respeita o encoding declarado):
                //   byte[] bytesPdf = gerador.Gerar(File.ReadAllBytes(xml));

                // --- Forma 2: direto de arquivo para arquivo -------------------------
                //   DanfseGenerator.GerarArquivo(xml, pdf);

                // --- Forma 3: com configuracao --------------------------------------
                //   var gerador = new DanfseGenerator
                //   {
                //       MunicipiosIbge = MinhaTabelaIbge,   // IDictionary<string,string>
                //       UrlQrCode = "https://www.nfse.gov.br/ConsultaPublica?tpc=1&chave={0}",
                //       Logotipo = File.ReadAllBytes("logo.png")
                //   };
                //   byte[] bytesPdf = gerador.Gerar(conteudoXml);

                Console.WriteLine("PDF gerado: " + Path.GetFullPath(pdf) +
                                  "  (" + bytesPdf.Length.ToString("N0") + " bytes)");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Falha ao gerar o DANFSe: " + ex.Message);
                Console.Error.WriteLine(ex);
                return 2;
            }
        }
    }
}
