namespace MonsterTradingCardsGame.BLL.Exceptions
{
    [Serializable]
    public class NoPackagesAvailableException : Exception
    {
        public NoPackagesAvailableException()
        {
        }

        public NoPackagesAvailableException(string? message) : base(message)
        {
        }

        public NoPackagesAvailableException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
