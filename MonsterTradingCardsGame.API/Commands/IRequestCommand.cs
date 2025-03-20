using MonsterTradingCardsGame.API.Server;

namespace MonsterTradingCardsGame.API.Commands
{
    public interface IRequestCommand
    {
        HttpResponse Execute(HttpRequest request);
    }
}