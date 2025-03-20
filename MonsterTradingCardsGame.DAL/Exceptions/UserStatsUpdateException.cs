namespace MonsterTradingCardsGame.DAL.Exceptions
{
    [Serializable]
    public class UserStatsUpdateException : Exception
    {
        public UserStatsUpdateException()
        {
        }

        public UserStatsUpdateException(string? message) : base(message)
        {
        }

        public UserStatsUpdateException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}