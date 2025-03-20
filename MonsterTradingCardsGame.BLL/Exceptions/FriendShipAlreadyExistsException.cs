namespace MonsterTradingCardsGame.BLL.Exceptions
{
    [Serializable]
    public class FriendShipAlreadyExistsException : Exception
    {
        public FriendShipAlreadyExistsException()
        {
        }

        public FriendShipAlreadyExistsException(string? message) : base(message)
        {
        }

        public FriendShipAlreadyExistsException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}