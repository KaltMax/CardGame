namespace MonsterTradingCardsGame.BLL.Exceptions
{
    [Serializable]
    public class AcquirePackageException : Exception
    {
        public AcquirePackageException()
        {
        }

        public AcquirePackageException(string? message) : base(message)
        {
        }

        public AcquirePackageException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}