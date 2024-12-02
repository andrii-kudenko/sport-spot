using Microsoft.AspNetCore.Mvc;
using SportSpot.Services.Interfaces;

namespace SportSpot.Operations.Controllers
{
    /*  Author: Danylo Chystov
        Description: Controller for handling notifications
    */
    public class NotificationController : Controller
    {
        private readonly INotificationInterface _notificationService;

        public NotificationController(INotificationInterface notificationService) 
        {
            _notificationService = notificationService;
        }

        //method to retrieve notifications that will be displayed on the page
        public async Task<IActionResult> Notifications()
        {
            //validation that user is logged in
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            //fetch all notifications
            var notifications = await _notificationService.GetNotificationsAsync(userId.Value);

            return View(notifications);
        }

        //method to mark all notifications as seen
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

