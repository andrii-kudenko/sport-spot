using System;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SportSpot.Entities.Models;
using SportSpot.Operations.Models;
using SportSpot.Services.Data;
using SportSpot.Services.Interfaces;

namespace SportSpot.Services.Services
{
    public class UserService : IUserInterface
    {
        private readonly SportsDbContext _context;
        

        public UserService(SportsDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetUsersByIdsAsync(List<int> userIds)
        {
            if (userIds == null || !userIds.Any())
            {
                return new List<User>(); // Return an empty list if the input is null or empty
            }

            return await _context.Users
                .Where(user => userIds.Contains(user.Id))
                .ToListAsync(); // Fetch users whose IDs are in the list
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        

        public async Task<User> UpdateUserAsync(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<List<User>> SearchUsersBySportAsync(Sports sport)
        {
            return await _context.Users
                .Where(u => u.Sports == sport)
                .ToListAsync();
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            Console.WriteLine($"Getting user by email: {email}");
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == email);
                Console.WriteLine($"User found: {user != null}");
                if (user != null)
                {
                    Console.WriteLine($"User details - ID: {user.Id}, Name: {user.Name}, HasPassword: {!string.IsNullOrEmpty(user.Password)}");
                }
                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user by email: {ex.Message}");
                throw;
            }
        }

        public async Task<List<User>> SearchUsersByCityAsync(string city)
        {
            return await _context.Users
                .Where(u => u.City.Contains(city))
                .ToListAsync();
        }

        public async Task<User> CreateUserAsync(User user)
        {
            try
            {
                Console.WriteLine($"Creating user with email: {user.Email}");
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                Console.WriteLine($"User created successfully with ID: {user.Id}");
                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating user: {ex.Message}");
                throw;
            }
        }

        /*
            Author: Andrii Kudenko
            Description: Get Filtered Users
            Parameter: String query, int excludeId for the user not to query himself
            Return: Filtered list of users
         */
        public async Task<List<User>> GetUsersByQuery(string query, int? excludeId)
        {
            Console.WriteLine($"{query} {excludeId}");
            var results =  _context.Users
                .Where(u => u.Id != excludeId)
                .AsEnumerable() // Switch to client-side evaluation
                .Where(u => u.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                            u.Email.Substring(0, u.Email.IndexOf('@')).Contains(query, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(u => u.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ThenByDescending(u => u.Email.Substring(0, u.Email.IndexOf('@')).Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return results;

        }

        public async Task<OperationResult> SendInviteRequestAsync(int eventId, int targetUserId)
        {
            var targetUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == targetUserId);
            if (targetUser == null)
            {
                return new OperationResult { Success = false, Message = "Target user not found." };
            }

            if (targetUser.Invintations.Contains(eventId))
            {
                return new OperationResult { Success = false, Message = "Invintation already sent." };
            }

            targetUser.Invintations.Add(eventId);
            await _context.SaveChangesAsync();
            return new OperationResult { Success = true, Message = "Invintation sent successfully." };
        }
        public async Task<OperationResult> AcceptInvintationAsync(int userId, int eventId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var targetEvent = await _context.Events.FirstOrDefaultAsync(u => u.Id == eventId);

            if (user == null || targetEvent == null)
            {
                return new OperationResult { Success = false, Message = "User not found." };
            }

            if (!user.FriendRequests.Contains(eventId))
            {
                return new OperationResult { Success = false, Message = "No invintations for this event." };
            }
            if (targetEvent.RegisteredPlayers == null)
                targetEvent.RegisteredPlayers = new List<User>();

           
            targetEvent.RegisteredPlayers.Add(user);                
            user.Invintations.Remove(eventId);            
            await _context.SaveChangesAsync();
            return new OperationResult { Success = true, Message = "Invintation accepted." };
        }

        public async Task<OperationResult> DeclineInvintationAsync(int userId, int eventId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return new OperationResult { Success = false, Message = "User not found." };
            }

            if (!user.Invintations.Contains(eventId))
            {
                return new OperationResult { Success = false, Message = "No invintation for this event." };
            }

            user.Invintations.Remove(eventId);
            await _context.SaveChangesAsync();
            return new OperationResult { Success = true, Message = "Invintation declined." };
        }
        public async Task<List<Event>> GetPendingInvintationAsync(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return new List<Event>();

            return await _context.Events
                .Where(u => user.Invintations.Contains(u.Id))
                .ToListAsync();
        }



        public async Task<OperationResult> SendFriendRequestAsync(int userId, int targetUserId)
        {
            var targetUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == targetUserId);
            if (targetUser == null)
            {
                return new OperationResult { Success = false, Message = "Target user not found." };
            }

            if (targetUser.FriendRequests.Contains(userId))
            {
                return new OperationResult { Success = false, Message = "Friend request already sent." };
            }

            targetUser.FriendRequests.Add(userId);
            await _context.SaveChangesAsync();
            return new OperationResult { Success = true, Message = "Friend request sent successfully." };
        }

        public async Task<OperationResult> AcceptFriendRequestAsync(int userId, int requesterId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var requester = await _context.Users.FirstOrDefaultAsync(u => u.Id == requesterId);

            if (user == null || requester == null)
            {
                return new OperationResult { Success = false, Message = "User not found." };
            }

            if (!user.FriendRequests.Contains(requesterId))
            {
                return new OperationResult { Success = false, Message = "No friend request from this user." };
            }

            user.FriendRequests.Remove(requesterId);
            user.Friends.Add(requesterId);
            requester.Friends.Add(userId);

            await _context.SaveChangesAsync();
            return new OperationResult { Success = true, Message = "Friend request accepted." };
        }

        public async Task<OperationResult> DeclineFriendRequestAsync(int userId, int requesterId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return new OperationResult { Success = false, Message = "User not found." };
            }

            if (!user.FriendRequests.Contains(requesterId))
            {
                return new OperationResult { Success = false, Message = "No friend request from this user." };
            }

            user.FriendRequests.Remove(requesterId);
            await _context.SaveChangesAsync();
            return new OperationResult { Success = true, Message = "Friend request declined." };
        }

        public async Task<OperationResult> RemoveFriendAsync(int userId, int friendId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var friend = await _context.Users.FirstOrDefaultAsync(u => u.Id == friendId);

            if (user == null || friend == null)
            {
                return new OperationResult { Success = false, Message = "User not found." };
            }

            if (!user.Friends.Contains(friendId))
            {
                return new OperationResult { Success = false, Message = "This user is not your friend." };
            }

            user.Friends.Remove(friendId);
            friend.Friends.Remove(userId);

            await _context.SaveChangesAsync();
            return new OperationResult { Success = true, Message = "Friend removed successfully." };
        }
        public async Task<List<User>> GetFriendsAsync(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return new List<User>();

            return await _context.Users
                .Where(u => user.Friends.Contains(u.Id))
                .ToListAsync();
        }
        public async Task<List<User>> GetPendingFriendRequestsAsync(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return new List<User>();

            return await _context.Users
                .Where(u => user.FriendRequests.Contains(u.Id))
                .ToListAsync();
        }

        
    }
}

    

        

        

