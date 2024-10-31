using CotadorAcoes.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace CotadorAcoes.Application.Services
{
    public class CotacaoManagerService : ICotacaoManagerService
    {
        private readonly ICotadorAcoesService _cotadorAcoesService;
        private readonly IEmailService _emailService;

        public CotacaoManagerService(ICotadorAcoesService cotadorAcoesService, IEmailService emailService)
        {
            _cotadorAcoesService = cotadorAcoesService ?? throw new ArgumentNullException(nameof(cotadorAcoesService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public async Task MonitorarCotacaoAsync(string ticker, decimal precoVendaReferencia, decimal precoCompraReferencia)
        {
            try
            {
                decimal cotacaoAtual = await _cotadorAcoesService.GetStockQuoteAsync(ticker);
                Console.WriteLine($"Cotação atual do ativo {ticker}: {cotacaoAtual}");

                if (cotacaoAtual > precoVendaReferencia)
                {
                    string emailBody = CreateEmailBody(ticker, cotacaoAtual, precoVendaReferencia, precoCompraReferencia, "venda");
                    _emailService.SendAlertEmail($"Aconselhamento de venda para {ticker}", emailBody);
                }
                else if (cotacaoAtual < precoCompraReferencia)
                {
                    string emailBody = CreateEmailBody(ticker, cotacaoAtual, precoVendaReferencia, precoCompraReferencia, "compra");
                    _emailService.SendAlertEmail($"Aconselhamento de compra para {ticker}", emailBody);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter a cotação ou enviar o e-mail: {ex.Message}");
            }
        }

        private string CreateEmailBody(string ativo, decimal cotacaoAtual, decimal precoVenda, decimal precoCompra, string tipoAlerta)
        {
            string tituloAlerta = tipoAlerta == "compra" ? "É hora de comprar" : "É hora de vender";
            string corTitulo = "#FFC107";
            string mensagemAlerta = tipoAlerta == "compra"
                ? "A ação está abaixo do preço configurado para compra. Esta pode ser uma boa oportunidade para comprar!"
                : "A ação está acima do preço configurado para venda. Esta pode ser uma boa oportunidade para vender!";

            return $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2 style='color: {corTitulo};'>{tituloAlerta} {ativo}!</h2>
                    <p>Olá,</p>
                    <p>Estamos monitorando a ação <strong>{ativo}</strong> e temos uma recomendação importante para você:</p>
                    <ul>
                        <li>Cotação atual: <strong>{cotacaoAtual:C}</strong></li>
                        <li>Preço de referência {(tipoAlerta == "compra" ? "de compra" : "de venda")}: <strong>{(tipoAlerta == "compra" ? precoCompra : precoVenda):C}</strong></li>
                    </ul>
                    <p>{mensagemAlerta}</p>
                    <p>Horário da verificação: {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}</p>
                    <br/>
                    <p>Este é um alerta gerado automaticamente.</p>
                    <p><em>Atenciosamente,</em><br/>Sistema de Monitoramento de Cotações</p>
                </body>
                </html>";
        }
    }
}
