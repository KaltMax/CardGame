namespace MonsterTradingCardsGame.API.Exceptions
{
    [Serializable]
    public class InvalidPackageDataException : Exception
    {
        public InvalidPackageDataException()
        {
        }

        public InvalidPackageDataException(string? message) : base(message)
        {
        }

        public InvalidPackageDataException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
