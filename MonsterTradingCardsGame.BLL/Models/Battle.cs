namespace MonsterTradingCardsGame.BLL.Models
{
    public class Battle
    {
        public Dictionary<string, string> BattleLog { get; init; }
        public User Player1 { get; init; }
        public User Player2 { get; init; }
        public Rules BattleRules { get; init; }

        private readonly Random _random = new();

        public Battle(User player1, User player2)
        {
            BattleLog = new Dictionary<string, string>();
            Player1 = player1;
            Player2 = player2;
            BattleRules = new Rules();
        }
        public void StartBattle()
        {
            // Start the BattleLog with the player names and their decks
            AddInitialBattleLog();

            var round = 0;

            while (Player1.Deck.PlayerDeck.Count > 0 && Player2.Deck.PlayerDeck.Count > 0 && round < 100)
            {
                round++;
                var player1Card = DrawRandomCard(Player1.Deck.PlayerDeck);
                var player2Card = DrawRandomCard(Player2.Deck.PlayerDeck);

                var (winner, logEntry) = BattleRules.DetermineRoundWinner(player1Card, player2Card, Player1, Player2);

                if (winner != null)
                    UpdatePlayerDecksAfterRound(winner, player1Card, player2Card);

                AddBattleLog($"Round {round}", logEntry);
            }

            // Determine winner of the battle
            var winnerFinal = DetermineWinner();

            // Update the player stats
            UpdatePlayerStatsAfterBattle(winnerFinal);

            // Add final battle log
            AddFinalBattleLog(winnerFinal);
        }

        public void UpdatePlayerDecksAfterRound(User winner, Card player1Card, Card player2Card)
        {
            // Update decks based on winner
            if (winner == Player1)
            {
                Player1.Deck.AddCard(player2Card);
                Player2.Deck.RemoveCard(player2Card);
            }
            else if (winner == Player2)
            {
                Player2.Deck.AddCard(player1Card);
                Player1.Deck.RemoveCard(player1Card);
            }
        }

        public User? DetermineWinner()
        {
            if (Player1.Deck.PlayerDeck.Count == 0 && Player2.Deck.PlayerDeck.Count == 0)
                return null; // Both decks are empty, it's a draw

            if (Player1.Deck.PlayerDeck.Count > 0 && Player2.Deck.PlayerDeck.Count == 0)
                return Player1; // Player2's deck is empty, Player1 wins

            if (Player2.Deck.PlayerDeck.Count > 0 && Player1.Deck.PlayerDeck.Count == 0)
                return Player2; // Player1's deck is empty, Player2 wins

            return null; // Both players still have cards, it's a draw
        }

        public void AddBattleLog(string key, string logEntry)
        {
            if (!BattleLog.TryAdd(key, logEntry))
                BattleLog[key] += $"\n{logEntry}"; // Append to existing log for the same key
        }

        public void UpdatePlayerStatsAfterBattle(User? winner)
        {
            if (winner == null)
                return; // Draw: Elo stays unchanged

            var loser = winner == Player1 ? Player2 : Player1;

            // Update stats
            winner.UpdateStats(true);
            loser.UpdateStats(false);
        }

        public void AddInitialBattleLog()
        {
            AddBattleLog("Battle", $"{Player1.Name} (Elo: {Player1.Elo}) versus {Player2.Name} (Elo: {Player2.Elo})");

            AddBattleLog("DecksBeforeBattle", $"{Player1.Name}'s Deck:\n" +
                                              string.Join(",\n", Player1.Deck.PlayerDeck.Select(c => $"{c.Name} (Damage: {c.Damage}, Element: {c.Element})")) +
                                              $"\n\n{Player2.Name}'s Deck: \n" +
                                              string.Join(",\n", Player2.Deck.PlayerDeck.Select(c => $"{c.Name} (Damage: {c.Damage}, Element: {c.Element})")));

        }

        public void AddFinalBattleLog(User? winnerFinal)
        {
            AddBattleLog("DecksAfterBattle", $"{Player1.Name}'s Deck:\n" +
                                              string.Join(",\n", Player1.Deck.PlayerDeck.Select(c => $"{c.Name} (Damage: {c.Damage}, Element: {c.Element})")) +
                                              $"\n\n{Player2.Name}'s Deck: \n" +
                                              string.Join(",\n", Player2.Deck.PlayerDeck.Select(c => $"{c.Name} (Damage: {c.Damage}, Element: {c.Element})")));

            if (winnerFinal == Player1)
                AddBattleLog("Result", $"{Player1.Name} (New Elo: {Player1.Elo}) won the battle against {Player2.Name} (New Elo: {Player2.Elo})!");
            else if (winnerFinal == Player2)
                AddBattleLog("Result", $"{Player2.Name} ({Player2.Elo}) won the battle against {Player1.Name} (New Elo: {Player1.Elo})!");
            else
                AddBattleLog("Result", "The battle ended in a draw. Elos stay unchanged.");
        }

        private Card DrawRandomCard(List<Card> deck)
        {
            var index = _random.Next(deck.Count);
            return deck[index];
        }
    }
}
