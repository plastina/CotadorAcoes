using CotadorAcoes.Configuration;
using CotadorAcoes.Services;
using Newtonsoft.Json;
using System;
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

        if (config.Api == null || string.IsNullOrEmpty(config.Api.BaseUrl) || string.IsNullOrEmpty(config.Api.ApiKey))
        {
            Console.WriteLine("Erro ao obter a cotação: Configuração da API está ausente ou incompleta.");
            return;
        }

        using var httpClient = new HttpClient();

        var cotadorAcoesService = new CotadorAcoesService(httpClient, config);
        var emailService = new EmailService(config);

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
                decimal precoVenda = Convert.ToDecimal(args[1]);
                decimal precoCompra = Convert.ToDecimal(args[2]);

                decimal cotacaoAtual = await cotadorAcoesService.GetStockQuoteAsync(ticker);
                Console.WriteLine($"Cotação atual do ativo {ticker}: {cotacaoAtual}");

                if (cotacaoAtual >= precoVenda)
                {
                    emailService.SendAlertEmail($"Alerta de venda para {ticker}", $"A cotação atual é {cotacaoAtual}, atingiu ou superou o preço de venda {precoVenda}");
                }
                if (cotacaoAtual <= precoCompra)
                {
                    emailService.SendAlertEmail($"Alerta de compra para {ticker}", $"A cotação atual é {cotacaoAtual}, atingiu ou ficou abaixo do preço de compra {precoCompra}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter a cotação ou enviar o e-mail: {ex.Message}");
            }

            Console.WriteLine("Aguardando 5 minutos para a próxima verificação...");
            await Task.Delay(TimeSpan.FromSeconds(10));
        }
    }
}