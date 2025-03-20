namespace MonsterTradingCardsGame.API.Server
{
    public class HttpRequest
    {
        public HttpMethod Method { get; set; }
        public string ResourcePath { get; set; }
        public string HttpVersion { get; set; }
        public Dictionary<string, string> Header { get; set; }
        public string? Payload { get; set; }

        public HttpRequest()
        {
            Method = HttpMethod.Get;
            ResourcePath = "";
            HttpVersion = "HTTP/1.1";
            Header = [];
            Payload = null;
        }
    }
}
