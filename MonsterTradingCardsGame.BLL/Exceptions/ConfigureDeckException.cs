namespace MonsterTradingCardsGame.BLL.Exceptions
{
    [Serializable]
    public class ConfigureDeckException : Exception
    {
        public ConfigureDeckException()
        {
        }

        public ConfigureDeckException(string? message) : base(message)
        {
        }

        public ConfigureDeckException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}