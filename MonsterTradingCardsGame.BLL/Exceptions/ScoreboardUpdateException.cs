namespace MonsterTradingCardsGame.BLL.Exceptions
{
    [Serializable]
    public class ScoreboardUpdateException : Exception
    {
        public ScoreboardUpdateException()
        {
        }

        public ScoreboardUpdateException(string? message) : base(message)
        {
        }

        public ScoreboardUpdateException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}