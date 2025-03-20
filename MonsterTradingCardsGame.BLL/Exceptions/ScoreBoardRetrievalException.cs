namespace MonsterTradingCardsGame.BLL.Exceptions
{
    [Serializable]
    public class ScoreboardRetrievalException : Exception
    {
        public ScoreboardRetrievalException()
        {
        }

        public ScoreboardRetrievalException(string? message) : base(message)
        {
        }

        public ScoreboardRetrievalException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}