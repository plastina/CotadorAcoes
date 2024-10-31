using System.Threading.Tasks;

namespace CotadorAcoes.Domain.Interfaces
{
    public interface ICotacaoManagerService
    {
        Task MonitorarCotacaoAsync(string ticker, decimal precoVendaReferencia, decimal precoCompraReferencia);
    }
}
