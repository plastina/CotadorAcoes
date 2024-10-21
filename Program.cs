using CotadorAcoes.Configuration;
using CotadorAcoes.Services;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

internal class Program
{
    public static AppSettings LoadAppSettings()
    {
        try
        {
            string basePath = AppContext.BaseDirectory;
            string configPath = Path.Combine(basePath, "Configuration", "appsettings.json");
            Console.WriteLine($"Carregando configurações de: {configPath}");

            string json = File.ReadAllText(configPath);
            return JsonConvert.DeserializeObject<AppSettings>(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao carregar as configurações: {ex.Message}");
            return null;
        }
    }

    private static async Task Main(string[] args)
    {
        AppSettings config = LoadAppSettings();

        if (config == null)
        {
            Console.WriteLine("Erro: Configurações não foram carregadas.");
            return;
        }

        if (config.Api == null || string.IsNullOrEmpty(config.Api.UrlPadrao) || string.IsNullOrEmpty(config.Api.ApiKey))
        {
            Console.WriteLine("Erro ao obter a cotação: Configuração da API está ausente ou incompleta.");
            return;
        }

        using HttpClient httpClient = new HttpClient();

        CotadorAcoesService cotadorAcoesService = new CotadorAcoesService(httpClient, config);
        EmailService emailService = new EmailService(config);

        while (true)
        {
            try
            {
                if (args.Length < 3)
                {
                    Console.WriteLine("Uso correto: <ativo> <preço de venda> <preço de compra>");
                    return;
                }

                string ticker = args[0];
                decimal precoVendaReferencia = Convert.ToDecimal(args[1], CultureInfo.InvariantCulture);
                decimal precoCompraReferencia = Convert.ToDecimal(args[2], CultureInfo.InvariantCulture);

                decimal cotacaoAtual = await cotadorAcoesService.GetStockQuoteAsync(ticker);
                Console.WriteLine($"Cotação atual do ativo {ticker}: {cotacaoAtual}");

                if (cotacaoAtual > precoVendaReferencia)
                {
                    string emailBody = CreateEmailBody(ticker, cotacaoAtual, precoVendaReferencia, precoCompraReferencia, "venda");
                    emailService.SendAlertEmail($"Aconselhamento de venda para {ticker}", emailBody);
                }
                else if (cotacaoAtual < precoCompraReferencia)
                {
                    string emailBody = CreateEmailBody(ticker, cotacaoAtual, precoVendaReferencia, precoCompraReferencia, "compra");
                    emailService.SendAlertEmail($"Aconselhamento de compra para {ticker}", emailBody);
                }
                else
                {
                    Console.WriteLine("Nenhum alerta disparado, o preço está dentro da faixa especificada.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter a cotação ou enviar o e-mail: {ex.Message}");
            }

            Console.WriteLine("Aguardando 5 minutos para a próxima verificação...");
            await Task.Delay(TimeSpan.FromMinutes(1));
        }
    }

    private static string CreateEmailBody(string ativo, decimal cotacaoAtual, decimal precoVenda, decimal precoCompra, string tipoAlerta)
    {
        string tituloAlerta = tipoAlerta == "compra" ? "É hora de comprar!" : "É hora de vender!";
        string corTitulo = "#FFC107";
        string mensagemAlerta = tipoAlerta == "compra"
            ? "A ação está abaixo do preço configurado para compra. Esta pode ser uma boa oportunidade para comprar!"
            : "A ação está acima do preço configurado para venda. Esta pode ser uma boa oportunidade para vender!";

        return $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2 style='color: {corTitulo};'>{tituloAlerta} para {ativo}</h2>
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