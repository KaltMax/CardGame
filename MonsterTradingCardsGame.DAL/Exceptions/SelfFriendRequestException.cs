namespace MonsterTradingCardsGame.DAL.Exceptions
{
    [Serializable]
    public class SelfFriendRequestException : Exception
    {
        public SelfFriendRequestException()
        {
        }

        public SelfFriendRequestException(string? message) : base(message)
        {
        }

        public SelfFriendRequestException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}