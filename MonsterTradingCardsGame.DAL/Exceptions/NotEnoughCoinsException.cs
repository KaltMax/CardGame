namespace MonsterTradingCardsGame.DAL.Exceptions
{
    [Serializable]
    public class NotEnoughCoinsException : Exception
    {
        public NotEnoughCoinsException()
        {
        }

        public NotEnoughCoinsException(string? message) : base(message)
        {
        }

        public NotEnoughCoinsException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}