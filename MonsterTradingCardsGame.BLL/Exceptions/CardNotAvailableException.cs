namespace MonsterTradingCardsGame.BLL.Exceptions
{
    [Serializable]
    public class CardNotAvailableException : Exception
    {
        public CardNotAvailableException()
        {
        }

        public CardNotAvailableException(string? message) : base(message)
        {
        }

        public CardNotAvailableException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}