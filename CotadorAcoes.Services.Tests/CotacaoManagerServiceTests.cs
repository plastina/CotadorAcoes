using CotadorAcoes.Managers;
using CotadorAcoes.Services;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

public class CotacaoManagerServiceTests
{
    private readonly CotacaoManagerService _cotacaoManagerService;
    private readonly Mock<CotadorAcoesService> _mockCotadorAcoesService;
    private readonly Mock<EmailService> _mockEmailService;

    public CotacaoManagerServiceTests()
    {
        _mockCotadorAcoesService = new Mock<CotadorAcoesService>();
        _mockEmailService = new Mock<EmailService>();
        _cotacaoManagerService = new CotacaoManagerService(_mockCotadorAcoesService.Object, _mockEmailService.Object);
    }

    [Fact]
    public async Task EnviarAlertaCotacaoUltrapassaReferenciaVenda()
    {
        string ticker = "AAPL";
        decimal precoVendaReferencia = 150m;
        decimal precoCompraReferencia = 100m;
        decimal cotacaoAtual = 160m;

        _mockCotadorAcoesService
            .Setup(service => service.GetStockQuoteAsync(ticker))
            .ReturnsAsync(cotacaoAtual);

        await _cotacaoManagerService.MonitorarCotacaoAsync(ticker, precoVendaReferencia, precoCompraReferencia);

        _mockEmailService.Verify(service => service.SendAlertEmail(
            It.Is<string>(subject => subject.Contains("venda")),
            It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task EnviarAlertaCotacaoUltrapassaReferenciaCompra()
    {
        string ticker = "AAPL";
        decimal precoVendaReferencia = 150m;
        decimal precoCompraReferencia = 100m;
        decimal cotacaoAtual = 90m;

        _mockCotadorAcoesService
            .Setup(service => service.GetStockQuoteAsync(ticker))
            .ReturnsAsync(cotacaoAtual);

        await _cotacaoManagerService.MonitorarCotacaoAsync(ticker, precoVendaReferencia, precoCompraReferencia);

        _mockEmailService.Verify(service => service.SendAlertEmail(
            It.Is<string>(subject => subject.Contains("compra")),
            It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task TratarExcecaoMonitorarCotacao()
    {
        string ticker = "AAPL";
        decimal precoVendaReferencia = 150m;
        decimal precoCompraReferencia = 100m;

        _mockCotadorAcoesService
            .Setup(service => service.GetStockQuoteAsync(ticker))
            .ThrowsAsync(new Exception("Erro ao obter cotação"));

        await Assert.ThrowsAsync<Exception>(() => _cotacaoManagerService.MonitorarCotacaoAsync(ticker, precoVendaReferencia, precoCompraReferencia));

        _mockEmailService.Verify(service => service.SendAlertEmail(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}