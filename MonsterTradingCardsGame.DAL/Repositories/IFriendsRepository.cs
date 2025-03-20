using MonsterTradingCardsGame.DTOs;

namespace MonsterTradingCardsGame.DAL.Repositories
{
    public interface IFriendsRepository
    {
        List<FriendEntryDTO> RetrieveFriends(string username);
        void CreateFriendRequest(string username, string targetUsername);
        void MarkFriendRequestAsAccepted(string username, string targetUsername);
        void DeleteFriendship(string username, string targetUsername);
    }
}
