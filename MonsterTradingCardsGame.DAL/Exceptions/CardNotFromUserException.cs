namespace MonsterTradingCardsGame.DAL.Exceptions
{
    [Serializable]
    public class CardNotFromUserException : Exception
    {
        public CardNotFromUserException()
        {
        }

        public CardNotFromUserException(string? message) : base(message)
        {
        }

        public CardNotFromUserException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}