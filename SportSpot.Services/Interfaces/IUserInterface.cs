using System;
using SportSpot.Entities.Models;
using SportSpot.Operations.Models;

namespace SportSpot.Services.Interfaces
{
    public interface IUserInterface
    {
        Task<User> GetUserByIdAsync(int id);
        Task<User> UpdateUserAsync(User user);
        Task<List<User>> SearchUsersBySportAsync(Sports sport);
        Task<List<User>> SearchUsersByCityAsync(string city);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> CreateUserAsync(User user);
        //Task<User> GetUserByEmailAsync(string email);  
        Task<List<User>> GetUsersByQuery(string query, int? excludeId);
        //"friends" functionality related,  by Danylo Chystov
        Task<OperationResult> SendFriendRequestAsync(int userId, int targetUserId);
        Task<OperationResult> AcceptFriendRequestAsync(int userId, int requesterId);
        Task<OperationResult> DeclineFriendRequestAsync(int userId, int requesterId);
        Task<List<User>> GetFriendsAsync(int userId);
        Task<List<User>> GetPendingFriendRequestsAsync(int userId);
        Task<List<User>> GetUsersByIdsAsync(List<int> userIds);
        Task<OperationResult> RemoveFriendAsync(int userId, int friendId);
    }
}

