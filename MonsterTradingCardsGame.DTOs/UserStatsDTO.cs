namespace MonsterTradingCardsGame.DTOs
{
    public class UserStatsDTO
    {
        public string Name { get; init; }
        public int Elo { get; init; }
        public int Wins { get; init; }
        public int Losses { get; init; }

        public UserStatsDTO(string name, int elo, int wins, int losses)
        {
            Name = name;
            Elo = elo;
            Wins = wins;
            Losses = losses;
        }

    }
}
