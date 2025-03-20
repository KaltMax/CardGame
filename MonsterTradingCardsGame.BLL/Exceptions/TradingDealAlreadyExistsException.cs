namespace MonsterTradingCardsGame.BLL.Exceptions
{
    [Serializable]
    public class TradingDealAlreadyExistsException : Exception
    {
        public TradingDealAlreadyExistsException()
        {
        }

        public TradingDealAlreadyExistsException(string? message) : base(message)
        {
        }

        public TradingDealAlreadyExistsException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}