using Microsoft.AspNetCore.Mvc;
using SportSpot.Services.Interfaces;

namespace SportSpot.Operations.Controllers
{
    public class NotificationController : Controller
    {
        private readonly INotificationInterface _notificationService;

        public NotificationController(INotificationInterface notificationService) 
        {
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Notifications()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var notifications = await _notificationService.GetNotificationsAsync(userId.Value);
            return View(notifications);
        }

        public async Task<IActionResult> MarkAllAsSeen()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            await _notificationService.MarkAllAsSeenAsync(userId.Value);
            return RedirectToAction("Notifications");
        }
    }
}

