namespace MonsterTradingCardsGame.BLL.Exceptions
{
    [Serializable]
    public class PlayerAlreadyInLobbyOrBattleException : Exception
    {
        public PlayerAlreadyInLobbyOrBattleException()
        {
        }

        public PlayerAlreadyInLobbyOrBattleException(string? message) : base(message)
        {
        }

        public PlayerAlreadyInLobbyOrBattleException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}