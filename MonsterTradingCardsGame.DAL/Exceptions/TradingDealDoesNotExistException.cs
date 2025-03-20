namespace MonsterTradingCardsGame.DAL.Exceptions
{
    [Serializable]
    public class TradingDealDoesNotExistException : Exception
    {
        public TradingDealDoesNotExistException()
        {
        }

        public TradingDealDoesNotExistException(string? message) : base(message)
        {
        }

        public TradingDealDoesNotExistException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}