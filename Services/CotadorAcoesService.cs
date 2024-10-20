using System;
using System.Net.Http;
using System.Threading.Tasks;
using CotadorAcoes.Models;
using CotadorAcoes.Configuration;
using Newtonsoft.Json;

namespace CotadorAcoes.Services
{
    public class CotadorAcoesService
    {
        private readonly HttpClient _httpClient;
        private readonly AppSettings _config;

        public CotadorAcoesService(HttpClient httpClient, AppSettings config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config), "Configuração não pode ser nula.");
            }

            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _config = config;
        }

        public async Task<decimal> GetStockQuoteAsync(string ticker)
        {
            if (_config.Api == null || string.IsNullOrEmpty(_config.Api.BaseUrl) || string.IsNullOrEmpty(_config.Api.ApiKey))
            {
                throw new Exception("Configuração da API está ausente ou incompleta.");
            }

            var url = $"{_config.Api.BaseUrl.TrimEnd('/')}/{ticker}?token={_config.Api.ApiKey}";

            Console.WriteLine("URL usada: " + url);

            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                throw new Exception("A URL formada é inválida: " + url);
            }

            var response = await _httpClient.GetAsync(new Uri(url));

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Erro ao obter a cotação: {response.StatusCode} ({response.ReasonPhrase})");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var cotadorAcoesResponse = JsonConvert.DeserializeObject<CotadorAcoesResponse>(jsonResponse);

            if (cotadorAcoesResponse.Results != null && cotadorAcoesResponse.Results.Length > 0)
            {
                return cotadorAcoesResponse.Results[0].RegularMarketPrice;
            }

            throw new Exception("Cotação não encontrada para o ativo: " + ticker);
        }
    }
}