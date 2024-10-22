# CotadorAcoes
O CotadorAcoes � uma aplica��o de monitoramento de a��es da Bolsa de Valores Brasileira (B3). Ele permite o acompanhamento de ativos financeiros em tempo real, verificando suas cota��es em intervalos regulares. A aplica��o dispara alertas por e-mail quando o pre�o de um ativo atinge um valor configurado para compra ou venda.

## Pr�-requisitos
.NET 6.0 SDK ou superior
Conta no Gmail ou outro servi�o SMTP configurado para enviar e-mails
Chave da API da Brapi (ou outra API que voc� estiver usando para buscar cota��es)

## Configura��o do Projeto
Instalar depend�ncias: Certifique-se de que todas as depend�ncias est�o instaladas corretamente, especialmente o Microsoft.Extensions.DependencyInjection para a inje��o de depend�ncias.
Configurar o arquivo appsettings.json: No diret�rio Configuration, voc� deve criar um arquivo appsettings.json com a estrutura abaixo:
{
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "Usuario": "seu-email@gmail.com",
    "Senha": "sua-senha-de-aplicativo",
    "UseSsl": true
  },
  "Email": {
    "Destinatario": "email-para-envio@exemplo.com"
  },
  "Api": {
    "UrlPadrao": "https://brapi.dev/api/quote",
    "ApiKey": "sua-chave-de-api"
  }
}

## Como Rodar a Aplica��o
Compilar e rodar: Para compilar e rodar o projeto, navegue at� o diret�rio raiz do projeto e execute o seguinte comando no terminal:
dotnet run <TICKER> <PRE�O_DE_VENDA> <PRE�O_DE_COMPRA>

TICKER: O s�mbolo do ativo que voc� deseja monitorar (ex: VALE3, PETR4, etc.).
PRE�O_DE_VENDA: O pre�o de venda desejado para gerar o alerta.
PRE�O_DE_COMPRA: O pre�o de compra desejado para gerar o alerta.

Exemplo:
dotnet run VALE3 80 60