namespace MonsterTradingCardsGame.API.Server
{
    public class HttpResponse
    {
        public StatusCode StatusCode { get; set; }
        public string? Payload { get; set; }
        public string ContentType { get; set; } = "text/plain; charset=utf-8"; // Default
    }
}
