using MonsterTradingCardsGame.DTOs;
using Npgsql;

namespace MonsterTradingCardsGame.DAL.Repositories
{
    public interface IPackageRepository
    {
        bool AddPackage(PackageDTO package);
        PackageDTO AcquirePackageAndDeductCoins(string username, decimal packagePrice);
    }
}
