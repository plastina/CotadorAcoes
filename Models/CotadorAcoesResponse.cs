namespace CotadorAcoes.Models
{
    public class CotadorAcoesResponse
    {
        public CotadorAcoesResult[] Results { get; set; }
    }

    public class CotadorAcoesResult
    {
        public string Symbol { get; set; }
        public decimal RegularMarketPrice { get; set; }
        public decimal RegularMarketDayHigh { get; set; }
        public decimal RegularMarketDayLow { get; set; }
    }
}