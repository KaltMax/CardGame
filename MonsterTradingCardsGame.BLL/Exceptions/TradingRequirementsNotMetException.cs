namespace MonsterTradingCardsGame.BLL.Exceptions
{
    [Serializable]
    public class TradingRequirementsNotMetException : Exception
    {
        public TradingRequirementsNotMetException()
        {
        }

        public TradingRequirementsNotMetException(string? message) : base(message)
        {
        }

        public TradingRequirementsNotMetException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}