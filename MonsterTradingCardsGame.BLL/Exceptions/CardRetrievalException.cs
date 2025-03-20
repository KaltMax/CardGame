namespace MonsterTradingCardsGame.BLL.Exceptions
{
    [Serializable]
    public class CardRetrievalException : Exception
    {
        public CardRetrievalException()
        {
        }

        public CardRetrievalException(string? message) : base(message)
        {
        }

        public CardRetrievalException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}