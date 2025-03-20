namespace MonsterTradingCardsGame.API.Exceptions
{
    [Serializable]
    public class InvalidDeckConfigurationException : Exception
    {
        public InvalidDeckConfigurationException()
        {
        }

        public InvalidDeckConfigurationException(string? message) : base(message)
        {
        }

        public InvalidDeckConfigurationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}