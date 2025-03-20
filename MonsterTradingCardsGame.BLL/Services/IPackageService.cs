using MonsterTradingCardsGame.DTOs;

namespace MonsterTradingCardsGame.BLL.Services
{
    public interface IPackageService
    {
        bool CreatePackage(PackageDTO package);
        PackageDTO AcquirePackage(string username);
    }
}
