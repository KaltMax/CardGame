using MonsterTradingCardsGame.DTOs;
using MonsterTradingCardsGame.DAL.Repositories;
using MonsterTradingCardsGame.BLL.Exceptions;
using MonsterTradingCardsGame.DAL.Exceptions;

namespace MonsterTradingCardsGame.BLL.Services
{
    public class PackageService : IPackageService
    {
        public int PackagePrice { get; init; }
        private readonly IPackageRepository _packageRepository;

        public PackageService(IPackageRepository packageRepository)
        {
            PackagePrice = 5;
            _packageRepository = packageRepository;
        }

        public bool CreatePackage(PackageDTO package)
        {
            var result = _packageRepository.AddPackage(package);
            if (!result)
            {
                throw new PackageCreationException("Failed to create package due to a database conflict.");
            }

            return true;
        }

        public PackageDTO AcquirePackage(string username)
        {
            try
            {
                // Proceed with acquiring the package if balance is sufficient
                return _packageRepository.AcquirePackageAndDeductCoins(username, PackagePrice);

            }
            catch (UserNotFoundException ex)
            {
                throw new UserDoesNotExistException("User not found", ex);
            }
            catch (PackageUnavailableException ex)
            {
                throw new NoPackagesAvailableException(ex.Message, ex);
            }
            catch (NotEnoughCoinsException ex)
            {
                throw new InsufficientFundsException("Not enough money for buying a card package", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AcquirePackageException("Failed to acquire package", ex);
            }
        }
    }
}
