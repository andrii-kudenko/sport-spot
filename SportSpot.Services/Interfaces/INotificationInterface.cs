using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportSpot.Entities.Models;

namespace SportSpot.Services.Interfaces
{
    public interface INotificationInterface
    {
        Task AddNotificationAsync(int userId, string message, string actionUrl);
        Task<List<Notification>> GetUnseenNotificationsAsync(int userId);
        Task<List<Notification>> GetNotificationsAsync(int userId);
        Task MarkAllAsSeenAsync(int userId);
    }
}
