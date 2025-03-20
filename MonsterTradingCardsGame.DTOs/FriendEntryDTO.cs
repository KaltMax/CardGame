namespace MonsterTradingCardsGame.DTOs
{
    public class FriendEntryDTO
    {
        public string FriendName { get; init; }
        public string Status { get; init; }

        public FriendEntryDTO(string friendName, string status)
        {
            FriendName = friendName;
            Status = status;
        }
    }
}