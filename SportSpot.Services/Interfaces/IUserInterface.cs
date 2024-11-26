using System;
using SportSpot.Entities.Models;

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
    }
}

