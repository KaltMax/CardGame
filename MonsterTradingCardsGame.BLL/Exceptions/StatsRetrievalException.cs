namespace MonsterTradingCardsGame.BLL.Exceptions
{
    [Serializable]
    public class StatsRetrievalException : Exception
    {
        public StatsRetrievalException()
        {
        }

        public StatsRetrievalException(string? message) : base(message)
        {
        }

        public StatsRetrievalException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}