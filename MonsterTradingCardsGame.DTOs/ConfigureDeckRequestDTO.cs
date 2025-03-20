namespace MonsterTradingCardsGame.DTOs
{
    public class ConfigureDeckRequestDTO
    {
        public List<string> CardIds { get; init; }

        public ConfigureDeckRequestDTO(List<string> cardIds)
        {
            CardIds = cardIds;
        }
    }
}