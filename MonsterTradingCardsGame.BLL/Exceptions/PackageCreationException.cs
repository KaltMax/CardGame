namespace MonsterTradingCardsGame.BLL.Exceptions
{
    [Serializable]
    public class PackageCreationException : Exception
    {
        public PackageCreationException()
        {
        }

        public PackageCreationException(string? message) : base(message)
        {
        }

        public PackageCreationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
