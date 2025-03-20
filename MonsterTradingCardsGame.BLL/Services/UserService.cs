using MonsterTradingCardsGame.BLL.Exceptions;
using MonsterTradingCardsGame.DTOs;
using MonsterTradingCardsGame.DAL.Repositories;
using MonsterTradingCardsGame.DAL.Exceptions;

namespace MonsterTradingCardsGame.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;


        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public bool RegisterUser(string username, string password)
        {
            try
            {
                // Hash the password
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

                // Save the hashed password to the database
                return _userRepository.AddUser(username, hashedPassword);
            }
            catch (UsernameAlreadyExistsException)
            {
                throw new UsernameAlreadyTaken("User already exists."); 
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while registering the user.", ex);
            }
        }

        public UserDataDTO GetUserData(string username)
        {
            try
            {
                return _userRepository.GetUserData(username);
            }
            catch (UserNotFoundException)
            {
                throw new UserDoesNotExistException("User does not exist.");
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving user data.", ex);
            }
        }

        public bool UpdateUserData(string username, UserDataDTO newUserData)
        {
            try
            {
                if (_userRepository.UpdateUserData(username, newUserData))
                {
                    return true;
                }

                throw new Exception("An error occurred while updating user data.");
            }
            catch (UserNotFoundException)
            {
                throw new UserDoesNotExistException();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating user data.", ex);
            }
        }
    }
}
