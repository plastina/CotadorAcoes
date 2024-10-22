using CotadorAcoes.Configuration;
using CotadorAcoes.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CotadorAcoes.Services
{
    public class CotadorAcoesService
    {
        private readonly HttpClient _httpClient;
        private readonly AppSettings _config;

        public CotadorAcoesService(HttpClient httpClient, AppSettings config)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _config = config ?? throw new ArgumentNullException(nameof(config), "Configuração não pode ser nula.");

            if (_config.Api == null || string.IsNullOrEmpty(_config.Api.UrlPadrao) || string.IsNullOrEmpty(_config.Api.ApiKey))
            {
                throw new ArgumentException("Configuração da API está ausente ou incompleta.");
            }
        }

        public async Task<decimal> GetStockQuoteAsync(string ticker)
        {
            if (_config.Api == null || string.IsNullOrEmpty(_config.Api.UrlPadrao) || string.IsNullOrEmpty(_config.Api.ApiKey))
            {
                throw new Exception("Configuração da API está ausente ou incompleta.");
            }

            string url = $"{_config.Api.UrlPadrao.TrimEnd('/')}/{ticker}?token={_config.Api.ApiKey}";

            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                throw new Exception("A URL formada é inválida: " + url);
            }

            HttpResponseMessage response = await _httpClient.GetAsync(new Uri(url));

            string jsonResponse = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Erro ao obter a cotação: {response.StatusCode} ({response.ReasonPhrase})");
            }

            CotadorAcoesResponse cotadorAcoesResponse = JsonConvert.DeserializeObject<CotadorAcoesResponse>(jsonResponse);

            if (cotadorAcoesResponse?.Results != null && cotadorAcoesResponse.Results.Length > 0)
            {
                return cotadorAcoesResponse.Results[0].RegularMarketPrice;
            }

            throw new Exception("Cotação não encontrada para o ativo: " + ticker);
        }
    }
}