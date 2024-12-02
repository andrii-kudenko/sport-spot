using Microsoft.EntityFrameworkCore;
using SportSpot.Entities.Models;
using SportSpot.Services.Data;
using SportSpot.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSpot.Services.Services
{
    public class NotificationService : INotificationInterface
    {
        private readonly SportsDbContext _context;

        public NotificationService(SportsDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task AddNotificationAsync(int userId, string message, string actionUrl)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                ActionUrl = actionUrl
            };
            _context.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Notification>> GetUnseenNotificationsAsync(int userId)
        {
            return await _context.Set<Notification>()
                .Where(n => n.UserId == userId && !n.IsSeen)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Notification>> GetNotificationsAsync(int userId)
        {
            return await _context.Set<Notification>()
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkAllAsSeenAsync(int userId)
        {
            var notifications = await _context.Set<Notification>()
                .Where(n => n.UserId == userId && !n.IsSeen)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsSeen = true;
            }

            await _context.SaveChangesAsync();
        }
    }
}
