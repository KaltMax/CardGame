namespace MonsterTradingCardsGame.DAL.Exceptions
{
    [Serializable]
    public class ForbiddenTradeException : Exception
    {
        public ForbiddenTradeException()
        {
        }

        public ForbiddenTradeException(string? message) : base(message)
        {
        }

        public ForbiddenTradeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}