namespace MonsterTradingCardsGame.DAL.Exceptions
{
    [Serializable]
    public class ForbiddenFriendException : Exception
    {
        public ForbiddenFriendException()
        {
        }

        public ForbiddenFriendException(string? message) : base(message)
        {
        }

        public ForbiddenFriendException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}