namespace MonsterTradingCardsGame.DAL.Exceptions
{
    [Serializable]
    public class TradingDealIdAlreadyTakenException : Exception
    {
        public TradingDealIdAlreadyTakenException()
        {
        }

        public TradingDealIdAlreadyTakenException(string? message) : base(message)
        {
        }

        public TradingDealIdAlreadyTakenException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}