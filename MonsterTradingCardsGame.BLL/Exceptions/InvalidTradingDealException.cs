namespace MonsterTradingCardsGame.BLL.Exceptions
{
    [Serializable]
    public class InvalidTradingDealException : Exception
    {
        public InvalidTradingDealException()
        {
        }

        public InvalidTradingDealException(string? message) : base(message)
        {
        }

        public InvalidTradingDealException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }

}