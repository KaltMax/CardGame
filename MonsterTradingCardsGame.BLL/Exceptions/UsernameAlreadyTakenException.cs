namespace MonsterTradingCardsGame.BLL.Exceptions
{
    [Serializable]
    public class UsernameAlreadyTaken : Exception
    {
        public UsernameAlreadyTaken()
        {
        }

        public UsernameAlreadyTaken(string? message) : base(message)
        {
        }

        public UsernameAlreadyTaken(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}