namespace MonsterTradingCardsGame.BLL.Models
{
    public class User
    {
        public string Name { get; private set; }
        public Deck Deck { get; private set; }
        public int Elo { get; private set; }
        public int Wins { get; private set; }
        public int Losses { get; private set; }
        
        public User(string username, Deck deck, int elo, int wins, int losses)
        {
            Name = username;
            Deck = deck;
            Elo = elo;
            Wins = wins;
            Losses = losses;
        }

        public void UpdateStats(bool isWinner)
        {
            if (isWinner)
            {
                Wins++;
                Elo += 3;
            }
            else
            {
                Losses++;
                Elo -= 5;

                if (Elo < 0)
                {
                    Elo = 0;
                }
            }
        }
    }
}
