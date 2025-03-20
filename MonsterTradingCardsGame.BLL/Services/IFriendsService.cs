using MonsterTradingCardsGame.DTOs;

namespace MonsterTradingCardsGame.BLL.Services
{
    public interface IFriendsService
    {
        List<FriendEntryDTO> GetFriendsList(string username);
        void SendFriendRequest(string username, string targetUsername);
        void AcceptFriendRequest(string username, string targetUsername);
        void RemoveFriend(string username, string targetUsername);
    }
}
