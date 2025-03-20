namespace MonsterTradingCardsGame.DAL.Exceptions
{
    [Serializable]
    public class PackageUnavailableException : Exception
    {
        public PackageUnavailableException()
        {
        }

        public PackageUnavailableException(string? message) : base(message)
        {
        }

        public PackageUnavailableException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
