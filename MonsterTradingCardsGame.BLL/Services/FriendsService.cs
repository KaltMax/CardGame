using MonsterTradingCardsGame.BLL.Exceptions;
using MonsterTradingCardsGame.DAL.Exceptions;
using MonsterTradingCardsGame.DAL.Repositories;
using MonsterTradingCardsGame.DTOs;

namespace MonsterTradingCardsGame.BLL.Services
{
    public class FriendsService : IFriendsService
    {
        private readonly IFriendsRepository _friendsRepository;

        public FriendsService(IFriendsRepository friendsRepository)
        {
            _friendsRepository = friendsRepository;
        }

        public List<FriendEntryDTO> GetFriendsList(string username)
        {
            try
            {
                return _friendsRepository.RetrieveFriends(username);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the friends list.", ex);
            }
        }

        public void SendFriendRequest(string username, string targetUsername)
        {
            try
            {
                _friendsRepository.CreateFriendRequest(username, targetUsername);
            }
            catch (SelfFriendRequestException ex)
            {
                throw new ForbiddenAccessException(ex.Message);
            }
            catch (DuplicateFriendRequestException ex)
            {
                throw new FriendShipAlreadyExistsException(ex.Message);
            }
            catch (UserNotFoundException ex)
            {
                throw new UserDoesNotExistException($"Friend request failed: {ex.Message}");
            }
            catch (DataAccessException ex)
            {
                throw new Exception("An error occurred while sending the friend request.", ex);
            }
        }

        public void AcceptFriendRequest(string username, string targetUsername)
        {
            try
            {
                _friendsRepository.MarkFriendRequestAsAccepted(username, targetUsername);
            }
            catch (FriendRequestNotFoundException ex)
            {
                throw new FriendRequestDoesNotExistException(ex.Message);
            }
            catch (ForbiddenFriendException ex)
            {
                throw new ForbiddenAccessException(ex.Message);
            }
            catch (UserNotFoundException ex)
            {
                throw new UserDoesNotExistException(ex.Message);
            }
            catch (DataAccessException ex)
            {
                throw new Exception("An error occurred while accepting the friend request.", ex);
            }
        }

        public void RemoveFriend(string username, string targetUsername)
        {
            try
            {
                _friendsRepository.DeleteFriendship(username, targetUsername);
            }
            catch (FriendRequestNotFoundException ex)
            {
                throw new FriendRequestDoesNotExistException(ex.Message);
            }
            catch (UserNotFoundException ex)
            {
                throw new UserDoesNotExistException(ex.Message);
            }
            catch (DataAccessException ex)
            {
                throw new Exception("An error occurred while removing the friend.", ex);
            }
        }
    }
}
