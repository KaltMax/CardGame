using MonsterTradingCardsGame.API.Server;

namespace MonsterTradingCardsGame.API.Routing
{
    public interface IRouter
    {
        public HttpResponse Route(HttpRequest request);
    }
}
