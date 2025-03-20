namespace MonsterTradingCardsGame.BLL.Exceptions
{
    [Serializable]
    public class InvalidFriendsRequestException : Exception
    {
        public InvalidFriendsRequestException()
        {
        }

        public InvalidFriendsRequestException(string? message) : base(message)
        {
        }

        public InvalidFriendsRequestException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }

}