namespace MonsterTradingCardsGame.DTOs
{
    public class CardDTO
    {
        public string Id { get; init; }
        public string Name { get; init; }
        public float Damage { get; init; }

        public CardDTO(string id, string name, float damage)
        {
            Id = id;
            Name = name;
            Damage = damage;
        }
    }
}
