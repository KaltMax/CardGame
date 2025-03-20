namespace MonsterTradingCardsGame.API.Exceptions
{
    [Serializable]
    public class InvalidFriendRequestDataException : Exception
    {
        public InvalidFriendRequestDataException()
        {
        }

        public InvalidFriendRequestDataException(string? message) : base(message)
        {
        }

        public InvalidFriendRequestDataException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}