namespace MonsterTradingCardsGame.BLL.Exceptions
{
    [Serializable]
    public class InsufficientFundsException : Exception
    {
        public InsufficientFundsException()
        {
        }

        public InsufficientFundsException(string? message) : base(message)
        {
        }

        public InsufficientFundsException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
