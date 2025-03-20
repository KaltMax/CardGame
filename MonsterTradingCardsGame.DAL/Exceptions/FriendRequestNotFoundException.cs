namespace MonsterTradingCardsGame.DAL.Exceptions
{
    [Serializable]
    public class FriendRequestNotFoundException : Exception
    {
        public FriendRequestNotFoundException()
        {
        }

        public FriendRequestNotFoundException(string? message) : base(message)
        {
        }

        public FriendRequestNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}