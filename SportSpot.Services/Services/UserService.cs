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
    }
}