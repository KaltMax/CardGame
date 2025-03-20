namespace MonsterTradingCardsGame.DAL.Exceptions
{
    [Serializable]
    public class CardDoesNotExistException : Exception
    {
        public CardDoesNotExistException()
        {
        }

        public CardDoesNotExistException(string? message) : base(message)
        {
        }

        public CardDoesNotExistException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}