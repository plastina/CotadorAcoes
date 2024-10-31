using CotadorAcoes.Application.Services;
using CotadorAcoes.Configuration;
using CotadorAcoes.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

internal class Program
{
    private static async Task Main(string[] args)
    {
        ServiceProvider serviceProvider = ConfigureServices();

        CotacaoManagerService cotacaoManagerService = serviceProvider.GetRequiredService<CotacaoManagerService>();

        if (args.Length < 3)
        {
            Console.WriteLine("Uso correto: <ativo> <preço de venda> <preço de compra>");
            return;
        }

        string ticker = args[0];
        decimal precoVendaReferencia = Convert.ToDecimal(args[1], CultureInfo.InvariantCulture);
        decimal precoCompraReferencia = Convert.ToDecimal(args[2], CultureInfo.InvariantCulture);

        while (true)
        {
            await cotacaoManagerService.MonitorarCotacaoAsync(ticker, precoVendaReferencia, precoCompraReferencia);

            Console.WriteLine("Aguardando 5 minutos para a próxima verificação...");
            await Task.Delay(TimeSpan.FromMinutes(5));
        }
    }

    private static ServiceProvider ConfigureServices()
    {
        ServiceCollection serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton<AppSettings>(sp => LoadAppSettings());
        serviceCollection.AddSingleton<HttpClient>();
        serviceCollection.AddSingleton<ICotadorAcoesService, CotadorAcoesService>();
        serviceCollection.AddSingleton<IEmailService, EmailService>();
        serviceCollection.AddSingleton<CotacaoManagerService>();

        return serviceCollection.BuildServiceProvider();
    }

    public static AppSettings LoadAppSettings()
    {
        try
        {
            string basePath = AppContext.BaseDirectory;
            string configPath = Path.Combine(basePath, "Configuration", "appsettings.json");
            Console.WriteLine($"Carregando configurações de: {configPath}");

            if (!File.Exists(configPath))
            {
                throw new FileNotFoundException("O arquivo de configuração appsettings.json não foi encontrado.");
            }

            string json = File.ReadAllText(configPath);
            Console.WriteLine("Conteúdo do arquivo appsettings.json:");
            Console.WriteLine(json);

            AppSettings config = JsonConvert.DeserializeObject<AppSettings>(json);
            if (config?.Api == null || string.IsNullOrEmpty(config.Api.UrlPadrao) || string.IsNullOrEmpty(config.Api.ApiKey))
            {
                throw new ArgumentException("Configuração da API está ausente ou incompleta.");
            }

            return config;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao carregar as configurações: {ex.Message}");
            return null;
        }
    }
}