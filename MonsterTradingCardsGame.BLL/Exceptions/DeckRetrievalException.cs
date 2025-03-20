namespace MonsterTradingCardsGame.BLL.Exceptions
{
    [Serializable]
    public class DeckRetrievalException : Exception
    {
        public DeckRetrievalException()
        {
        }

        public DeckRetrievalException(string? message) : base(message)
        {
        }

        public DeckRetrievalException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}