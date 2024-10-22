# CotadorAcoes
O CotadorAcoes é uma aplicação de monitoramento de ações da Bolsa de Valores Brasileira (B3). Ele permite o acompanhamento de ativos financeiros em tempo real, verificando suas cotações em intervalos regulares. A aplicação dispara alertas por e-mail quando o preço de um ativo atinge um valor configurado para compra ou venda.

## Pré-requisitos
.NET 6.0 SDK ou superior
Conta no Gmail ou outro serviço SMTP configurado para enviar e-mails
Chave da API da Brapi (ou outra API que você estiver usando para buscar cotações)

## Configuração do Projeto
Instalar dependências: Certifique-se de que todas as dependências estão instaladas corretamente, especialmente o Microsoft.Extensions.DependencyInjection para a injeção de dependências.
Configurar o arquivo appsettings.json: No diretório Configuration, você deve criar um arquivo appsettings.json com a estrutura abaixo:
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

## Como Rodar a Aplicação
Compilar e rodar: Para compilar e rodar o projeto, navegue até o diretório raiz do projeto e execute o seguinte comando no terminal:
dotnet run <TICKER> <PREÇO_DE_VENDA> <PREÇO_DE_COMPRA>

TICKER: O símbolo do ativo que você deseja monitorar (ex: VALE3, PETR4, etc.).
PREÇO_DE_VENDA: O preço de venda desejado para gerar o alerta.
PREÇO_DE_COMPRA: O preço de compra desejado para gerar o alerta.

Exemplo:
dotnet run VALE3 80 60