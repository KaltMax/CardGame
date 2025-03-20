using MonsterTradingCardsGame.DTOs;

namespace MonsterTradingCardsGame.DAL.Repositories
{
    public interface ITradingsRepository
    {
        void SaveTradingDeal(string username, TradingDealDTO tradingDeal);
        List<TradingDealDTO> GetAvailableTradingDeals();
        void DeleteTradingDeal(string username, string tradeId);
        TradingDealDTO GetTradingDealById(string tradeId);
        void TradingDealTransaction(string username, int buyerUserId, int offeringUserId, string tradingDealId,
            string offeredCardId, TradingDealDTO tradingDeal);
        int GetOfferingUserId(string tradingDealId);
        int GetUserId(string username);
    }
}
