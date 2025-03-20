namespace MonsterTradingCardsGame.BLL.Exceptions
{
    [Serializable]
    public class TradingDealNotFoundException : Exception
    {
        public TradingDealNotFoundException()
        {
        }

        public TradingDealNotFoundException(string? message) : base(message)
        {
        }

        public TradingDealNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}