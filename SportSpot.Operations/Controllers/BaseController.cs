using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SportSpot.Services.Interfaces;

namespace SportSpot.Operations.Controllers
{
    public class BaseController : Controller
    {
        protected readonly INotificationInterface _notificationService;

        public BaseController(INotificationInterface notificationService)
        {
            _notificationService = notificationService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userId = HttpContext.Session.GetInt32("UserId"); 
            if (userId != null)
            {
                var unseenCount = _notificationService.GetUnseenNotificationsAsync(userId.Value).Result.Count;
                ViewBag.UnseenNotificationsCount = unseenCount;
            }
            else
            {
                //if the user is not logged-in 0 notifications
                ViewBag.UnseenNotificationsCount = 0;
            }
            base.OnActionExecuting(context); 
        }
    }
}
