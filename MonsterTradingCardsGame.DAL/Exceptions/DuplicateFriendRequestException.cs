namespace MonsterTradingCardsGame.DAL.Exceptions
{
    [Serializable]
    public class DuplicateFriendRequestException : Exception
    {
        public DuplicateFriendRequestException()
        {
        }

        public DuplicateFriendRequestException(string? message) : base(message)
        {
        }

        public DuplicateFriendRequestException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}