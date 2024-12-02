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

        /*
            Author: Danylo Chystov
            Description: Adds new notification when called
            Parameter: int id of recepient, string message to be shown in notification, string url to page that shows on Action button click
            Return: none
         */
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

        /*
            Author: Danylo Chystov
            Description: Method used to get unseen notifications
            Parameter: int id of user (possibly any user, practically logged in user)
            Return: list of Notificstion type objects
         */
        public async Task<List<Notification>> GetUnseenNotificationsAsync(int userId)
        {
            return await _context.Set<Notification>()
                .Where(n => n.UserId == userId && !n.IsSeen)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        /*
            Author: Danylo Chystov
            Description: Fetches all notifications 
            Parameter: int id of user (possibly any user, practically logged in user)
            Return: list of notification type objects
         */
        public async Task<List<Notification>> GetNotificationsAsync(int userId)
        {
            return await _context.Set<Notification>()
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        /*
            Author: Danylo Chystov
            Description: Marks all notifications as seen(changes isSeen property) , for all notifications with matching to passed user id, and wich are not seen changes property of isSeen to true
            Parameter: int id of user (possibly any user, practically logged in user)
            Return: nothing
         */
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
