using System;
using Microsoft.EntityFrameworkCore;
using SportSpot.Entities.Models;
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

        public async Task<List<User>> GetUsersByQuery(string query, int? excludeId)
        {
            /*return await _context.Users.Where(u => u.Name.ToLower().Contains(query) || u.Email.ToLower().Contains(query))
                .OrderByDescending(u => u.Name.ToLower().Contains(query))
                .ThenByDescending(u => u.Email.ToLower().Contains(query))
                .ToListAsync();*/
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
    }
}