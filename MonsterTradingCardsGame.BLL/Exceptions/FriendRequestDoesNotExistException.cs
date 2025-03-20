namespace MonsterTradingCardsGame.BLL.Exceptions
{
    [Serializable]
    public class FriendRequestDoesNotExistException : Exception
    {
        public FriendRequestDoesNotExistException()
        {
        }

        public FriendRequestDoesNotExistException(string? message) : base(message)
        {
        }

        public FriendRequestDoesNotExistException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}