using MonsterTradingCardsGame.DTOs;

namespace MonsterTradingCardsGame.BLL.Services
{
    public interface ITradingsService
    {
        void CreateTradingDeal(string username, TradingDealDTO tradingDeal);
        List<TradingDealDTO> GetAvailableTradingDeals();
        void DeleteTradingDeal(string username, string tradingDealId);
        void ExecuteTradingDeal(string username, string tradingDealId, string offeredCardId);
    }
}
