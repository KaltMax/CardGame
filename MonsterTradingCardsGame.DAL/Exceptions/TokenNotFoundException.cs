namespace MonsterTradingCardsGame.DAL.Exceptions
{
    [Serializable]
    public class TokenNotFoundException : Exception
    {
        public TokenNotFoundException()
        {
        }

        public TokenNotFoundException(string? message) : base(message)
        {
        }

        public TokenNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
