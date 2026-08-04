# DANFSe v2.0 — geração de PDF a partir do XML da NFS-e Nacional

Biblioteca em **C# / .NET Framework 4.8** que gera o **DANFSe v2.0** (Documento Auxiliar
da NFS-e) em PDF a partir do XML da NFS-e Nacional (layout SEFIN Nacional, namespace
`http://www.sped.fazenda.gov.br/nfse`).

O layout foi reconstruído a partir do PDF oficial emitido pelo ambiente nacional:
coordenadas, fontes, alturas de linha, faixas cinza e traços foram extraídos do documento
de referência e reproduzidos ponto a ponto.

## Uso

### String do XML → bytes do PDF

Forma indicada quando o XML já está em memória (retorno de webservice, coluna de banco,
fila) e o destino do PDF não é necessariamente o disco. **O gerador não toca em disco:**
quem grava, anexa ou devolve o PDF é a aplicação chamadora.

```csharp
using SysValley.Danfse;

var gerador = new DanfseGenerator();
byte[] pdf = gerador.Gerar(xmlDaNota);      // xmlDaNota é uma string

File.WriteAllBytes(@"C:\nfse\nota.pdf", pdf);
```

Atalho estático, quando não há nada a configurar:

```csharp
byte[] pdf = DanfseGenerator.GerarDeXml(xmlDaNota);
```

O XML também pode ser passado **em bytes** — nesse caso a codificação declarada no prólogo
(`<?xml ... encoding="ISO-8859-1"?>`) é respeitada, o que evita acentuação corrompida:

```csharp
byte[] pdf = gerador.Gerar(bytesDoXml);
byte[] pdf = DanfseGenerator.GerarDeXml(bytesDoXml);   // atalho estático
```

### Arquivo → arquivo

```csharp
DanfseGenerator.GerarArquivo(@"C:\nfse\nota.xml", @"C:\nfse\nota.pdf");
```

### Com configuração

```csharp
var gerador = new DanfseGenerator
{
    MunicipiosIbge = minhaTabelaIbge,   // IDictionary<string,string>: "4202008" -> "Balneário Camboriú"
    UrlQrCode      = "https://www.nfse.gov.br/ConsultaPublica?tpc=1&chave={0}",
    Logotipo       = File.ReadAllBytes("logo.png")   // opcional; o padrão é a logo NFS-e embutida
};

byte[] pdf = gerador.Gerar(xmlDaNota);
```

### Resumo da API

| Membro | Entrada | Retorno |
|---|---|---|
| `gerador.Gerar(string xml)` | string do XML | `byte[]` do PDF |
| `gerador.Gerar(byte[] xml)` | XML em bytes | `byte[]` do PDF |
| `gerador.Gerar(NfseDocumento nfse)` | modelo já lido | `byte[]` do PDF |
| `DanfseGenerator.GerarDeXml(string xml)` | string do XML | `byte[]` do PDF |
| `DanfseGenerator.GerarDeXml(byte[] xml)` | XML em bytes | `byte[]` do PDF |
| `DanfseGenerator.GerarArquivo(xml, pdf)` | caminhos | grava o arquivo |

A mesma instância de `DanfseGenerator` pode ser reutilizada para várias notas.
Um BOM ou espaços antes do prólogo do XML são tolerados.

## Dependências (NuGet)

| Pacote     | Versão      | Licença |
|------------|-------------|---------|
| `PDFsharp` | 1.50.5147   | MIT     |
| `QRCoder`  | 1.4.3       | MIT     |

Ambos rodam nativamente em .NET Framework 4.8, sem custo de licença.
As fontes usadas (**Arial** e **Microsoft Sans Serif**) são padrão do Windows.

## Estrutura

| Arquivo | Responsabilidade |
|---|---|
| `DANFSe.sln`                              | Solução agrupando a biblioteca e o exemplo |
| `src/SysValley.Danfse/DanfseGenerator.cs` | Renderização do PDF (grade, blocos, paginação, QR Code) |
| `src/SysValley.Danfse/NfseDocumento.cs`   | Leitura do XML para um modelo tipado (independente de namespace) |
| `src/SysValley.Danfse/DanfseFormato.cs`   | Máscaras (CNPJ, CEP, IBGE, NBS, moeda) e domínios do layout nacional |
| `src/SysValley.Danfse/DanfseRecursos.cs`  | Logomarca NFS-e embutida em Base64 |
| `exemplo/Danfse.Exemplo`                  | Aplicação de linha de comando: `Danfse.Exemplo nota.xml [saida.pdf]` |

## O que a classe cobre

- Cabeçalho com logo, município emissor, ambiente gerador e tipo de ambiente.
- Chave de acesso, números/datas da NFS-e e da DPS, emitente, situação e finalidade.
- QR Code de consulta pública (mesma URL do portal nacional) e texto de autenticidade.
- Prestador, Tomador/Adquirente, Destinatário e Intermediário. Quando o grupo não existe
  no XML, imprime a faixa *"... NÃO IDENTIFICADO NA NFS-e"*, como no documento oficial.
- Serviço prestado, códigos de tributação nacional/municipal, NBS e descrição do serviço
  (respeitando as quebras de linha do XML e quebrando por largura quando necessário).
- Tributação municipal (ISSQN), federal (exceto CBS) e o bloco IBS/CBS da reforma.
- Totais, retenções e informações complementares (Lei 12.741/2012).
- **Paginação automática**: descrições longas quebram entre páginas; cabeçalho, moldura e
  rodapé são repetidos, e o rótulo não é reimpresso na continuação.

## Resolução de municípios

O nome do município do tomador não vem no XML — só o código IBGE. A classe resolve assim,
nesta ordem:

1. Códigos já citados no próprio XML (`cLocEmi`/`xLocEmi`, `cLocPrestacao`/`xLocPrestacao`,
   `cLocIncid`/`xLocIncid`) — cobre o caso comum de prestador e tomador no mesmo município.
2. A tabela informada em `MunicipiosIbge`.
3. Caso não encontre, imprime `-`.

A **sigla da UF** é sempre derivada dos dois primeiros dígitos do código IBGE, então não
depende de tabela externa.

## Fidelidade ao documento de referência

Comparando o PDF gerado com o DANFSe oficial do mesmo XML:

- **159 de 159** blocos de texto presentes, com desvio máximo de posição de **0,02 pt**.
- Faixas cinza, moldura, caixas do rodapé e posicionamento das imagens idênticos.
- QR Code decodifica exatamente a mesma URL.

Duas diferenças conhecidas, ambas intencionais:

1. **Corte com reticências.** No campo *"Simples Nacional na Data de Competência"* o texto
   não cabe na coluna e é cortado. O ponto de corte difere em alguns caracteres do gerador
   oficial (`...Individua...` × `...Individual (...`), por medir a largura da fonte de forma
   ligeiramente diferente. Ambos cabem na coluna.
2. **Traços separadores.** O gerador oficial desenha cada traço horizontal em segmentos por
   coluna; aqui é um traço único, no mesmo `y` e com a mesma espessura. O resultado impresso
   é idêntico.

## Pontos do layout que merecem atenção

Estes comportamentos foram copiados do gerador oficial e estão marcados por comentário no
código, caso a legislação ou o portal mudem:

- **"VALOR LÍQUIDO DA NFS-e + IBS/CBS"** e **"Total do IBS/CBS"** saem como `R$ 0,00`
  enquanto o XML não trouxer o grupo IBS/CBS — é o que o portal nacional imprime hoje, na
  transição da reforma tributária. Quando o grupo existir, o valor passa a ser
  `vLiq + IBS + CBS`. Ver `BlocoValorTotal`.
- **"Red. Alíquota IBS / Red. Alíquota CBS"** é impresso com *três* componentes
  (`- / - / -`) apesar de o rótulo citar apenas dois. É assim no documento oficial; mantido
  para não divergir. Ver `BlocoTributacaoIbsCbs`.
- **"Situação da NFS-e"** é derivada: cancelada → substituída → `NFS-e MEI` (quando
  `opSimpNac = 2`) → `NFS-e Normal`. Ver `DanfseFormato.SituacaoNfse`.

## Compilação

Abra `DANFSe.sln` no Visual Studio ou compile pela linha de comando:

```bash
dotnet build DANFSe.sln
```

Os `.csproj` são SDK-style com `TargetFramework=net48` (usam `PackageReference`). Para um
projeto .NET Framework clássico, basta copiar os quatro arquivos de
`src/SysValley.Danfse/` e instalar os dois pacotes NuGet.
