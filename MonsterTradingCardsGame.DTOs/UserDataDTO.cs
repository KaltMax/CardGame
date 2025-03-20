namespace MonsterTradingCardsGame.DTOs
{
    public class UserDataDTO
    {
        public string Name { get; init; }
        public string Bio { get; init; }
        public string Image { get; init; }

        public UserDataDTO(string name, string bio, string image)
        {
            Name = name;
            Bio = bio;
            Image = image;
        }
    }
}
