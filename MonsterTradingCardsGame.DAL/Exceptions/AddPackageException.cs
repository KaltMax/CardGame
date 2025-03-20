namespace MonsterTradingCardsGame.DAL.Exceptions
{
    [Serializable]
    public class AddPackageException : Exception
    {
        public AddPackageException()
        {
        }

        public AddPackageException(string? message) : base(message)
        {
        }

        public AddPackageException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}