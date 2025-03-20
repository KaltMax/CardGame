namespace MonsterTradingCardsGame.DTOs
{
    public class TokenDTO
    {
        public string Username { get; init; }
        public string Token { get; init; }

        public TokenDTO(string username, string token)
        {
            Username = username;
            Token = token;
        }
    }
}
