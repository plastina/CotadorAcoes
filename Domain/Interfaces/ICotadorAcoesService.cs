using System.Threading.Tasks;

namespace CotadorAcoes.Domain.Interfaces
{
    public interface ICotadorAcoesService
    {
        Task<decimal> GetStockQuoteAsync(string ticker);
    }
}
