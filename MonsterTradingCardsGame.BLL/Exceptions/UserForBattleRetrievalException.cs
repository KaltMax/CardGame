namespace MonsterTradingCardsGame.BLL.Exceptions
{
    [Serializable]
    public class UserForBattleRetrievalException : Exception
    {
        public UserForBattleRetrievalException()
        {
        }

        public UserForBattleRetrievalException(string? message) : base(message)
        {
        }

        public UserForBattleRetrievalException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}