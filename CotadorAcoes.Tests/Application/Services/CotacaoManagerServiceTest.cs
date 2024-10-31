using CotadorAcoes.Application.Services;
using CotadorAcoes.Domain.Interfaces;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CotadorAcoes.Tests
{
    public class CotacaoManagerServiceTests
    {
        private readonly Mock<ICotadorAcoesService> _cotadorAcoesServiceMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly CotacaoManagerService _cotacaoManagerService;

        public CotacaoManagerServiceTests()
        {
            _cotadorAcoesServiceMock = new Mock<ICotadorAcoesService>();
            _emailServiceMock = new Mock<IEmailService>();
            _cotacaoManagerService = new CotacaoManagerService(_cotadorAcoesServiceMock.Object, _emailServiceMock.Object);
        }

        [Fact]
        public async Task MonitorarCotacaoAsync_ShouldSendSellAlert_WhenCotacaoExceedsPrecoVenda()
        {
            string ticker = "TEST";
            decimal precoVendaReferencia = 100m;
            decimal precoCompraReferencia = 80m;
            decimal cotacaoAtual = 110m;

            _cotadorAcoesServiceMock
                .Setup(service => service.GetStockQuoteAsync(ticker))
                .ReturnsAsync(cotacaoAtual);

            await _cotacaoManagerService.MonitorarCotacaoAsync(ticker, precoVendaReferencia, precoCompraReferencia);

            _emailServiceMock.Verify(emailService => emailService.SendAlertEmail(
                It.Is<string>(subject => subject.Contains("venda")),
                It.Is<string>(body => body.Contains("venda"))
            ), Times.Once);
        }

        [Fact]
        public async Task MonitorarCotacaoAsync_ShouldSendBuyAlert_WhenCotacaoFallsBelowPrecoCompra()
        {
            string ticker = "TEST";
            decimal precoVendaReferencia = 100m;
            decimal precoCompraReferencia = 80m;
            decimal cotacaoAtual = 70m;

            _cotadorAcoesServiceMock
                .Setup(service => service.GetStockQuoteAsync(ticker))
                .ReturnsAsync(cotacaoAtual);

            await _cotacaoManagerService.MonitorarCotacaoAsync(ticker, precoVendaReferencia, precoCompraReferencia);

            _emailServiceMock.Verify(emailService => emailService.SendAlertEmail(
                It.Is<string>(subject => subject.Contains("compra")),
                It.Is<string>(body => body.Contains("compra"))
            ), Times.Once);
        }

        [Fact]
        public async Task MonitorarCotacaoAsync_ShouldNotSendEmail_WhenCotacaoWithinBounds()
        {
            string ticker = "TEST";
            decimal precoVendaReferencia = 100m;
            decimal precoCompraReferencia = 80m;
            decimal cotacaoAtual = 90m;

            _cotadorAcoesServiceMock
                .Setup(service => service.GetStockQuoteAsync(ticker))
                .ReturnsAsync(cotacaoAtual);

            await _cotacaoManagerService.MonitorarCotacaoAsync(ticker, precoVendaReferencia, precoCompraReferencia);

            _emailServiceMock.Verify(emailService => emailService.SendAlertEmail(
                It.IsAny<string>(),
                It.IsAny<string>()
            ), Times.Never);
        }

        [Fact]
        public async Task MonitorarCotacaoAsync_ShouldHandleException_WhenCotadorAcoesServiceThrows()
        {
            string ticker = "TEST";
            decimal precoVendaReferencia = 100m;
            decimal precoCompraReferencia = 80m;

            _cotadorAcoesServiceMock
                .Setup(service => service.GetStockQuoteAsync(ticker))
                .ThrowsAsync(new Exception("Erro de serviço"));

            Exception exception = await Record.ExceptionAsync(() => _cotacaoManagerService.MonitorarCotacaoAsync(ticker, precoVendaReferencia, precoCompraReferencia));
            Assert.Null(exception); 
        }
    }
}
