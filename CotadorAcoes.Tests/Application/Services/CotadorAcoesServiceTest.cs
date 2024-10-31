using CotadorAcoes.Application.Services;
using CotadorAcoes.Configuration;
using CotadorAcoes.Domain.Interfaces;
using CotadorAcoes.Models;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CotadorAcoes.Tests
{
    public class CotadorAcoesServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly AppSettings _appSettings;
        private readonly ICotadorAcoesService _cotadorAcoesService;

        public CotadorAcoesServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _appSettings = new AppSettings
            {
                Api = new ApiSettings
                {
                    UrlPadrao = "https://api.mockservice.com",
                    ApiKey = "mockApiKey"
                }
            };

            _cotadorAcoesService = new CotadorAcoesService(_httpClient, _appSettings);
        }

        [Fact]
        public async Task ObterCotacaoAcaoSucesso()
        {
            string ticker = "AAPL";
            decimal expectedPrice = 150.25m;
            string responseContent = JsonConvert.SerializeObject(new CotadorAcoesResponse
            {
                Results = new[] { new CotadorAcoesResult { RegularMarketPrice = expectedPrice } }
            });

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent)
                });

            decimal actualPrice = await _cotadorAcoesService.ObterCotacaoAsync(ticker);

            Assert.Equal(expectedPrice, actualPrice);
        }

        [Fact]
        public async Task ObterCotacaoAcaoSemSucesso()
        {
            string ticker = "AAPL";
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                    ReasonPhrase = "Not Found"
                });

            Exception exception = await Assert.ThrowsAsync<Exception>(() => _cotadorAcoesService.ObterCotacaoAsync(ticker));
            Assert.Contains("Erro ao obter a cotação", exception.Message);
        }

        [Fact]
        public async Task ObterCotacaoAcaoRespostaApiInvalida()
        {
            string ticker = "AAPL";
            string responseContent = "{}"; 

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent)
                });

            Exception exception = await Assert.ThrowsAsync<Exception>(() => _cotadorAcoesService.ObterCotacaoAsync(ticker));
            Assert.Contains("Cotação não encontrada para o ativo", exception.Message);
        }

        [Fact]
        public void ConstrutorLancamentoExceptionConfigNula()
        {
            Assert.Throws<ArgumentNullException>(() => new CotadorAcoesService(_httpClient, null));
        }

        [Fact]
        public void ConstrutorLancamentoExceptionConfigIncompleta()
        {
            AppSettings incompleteSettings = new AppSettings { Api = new ApiSettings { UrlPadrao = null, ApiKey = null } };

            Assert.Throws<ArgumentException>(() => new CotadorAcoesService(_httpClient, incompleteSettings));
        }

        [Fact]
        public async Task ObterCotacaoAcaoExcecaoUrlInvalida()
        {
            string ticker = "AAPL";
            _appSettings.Api.UrlPadrao = "invalid-url";

            Exception exception = await Assert.ThrowsAsync<Exception>(() => _cotadorAcoesService.ObterCotacaoAsync(ticker));
            Assert.Contains("A URL formada é inválida", exception.Message);
        }
    }
}
