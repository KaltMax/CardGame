namespace MonsterTradingCardsGame.DTOs
{
    public class TradingDealDTO
    {
        public string Id { get; init; }
        public string CardToTrade { get; init; }
        public string Type { get; init; }
        public float MinimumDamage { get; init; }

        public TradingDealDTO(string id, string cardToTrade, string type, float minimumDamage)
        {
            Id = id;
            CardToTrade = cardToTrade;
            Type = type;
            MinimumDamage = minimumDamage;
        }
    }
}
