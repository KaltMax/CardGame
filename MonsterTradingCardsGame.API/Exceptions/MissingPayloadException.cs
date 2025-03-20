namespace MonsterTradingCardsGame.API.Exceptions
{
    [Serializable]
    public class MissingPayloadException : Exception
    {
        public MissingPayloadException()
        {
        }

        public MissingPayloadException(string? message) : base(message)
        {
        }

        public MissingPayloadException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}