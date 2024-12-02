using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SportSpot.Services.Interfaces;

namespace SportSpot.Operations.Controllers
{
    /*  Author: Danylo Chystov
        Description: Controller that will act as a base 
        for any controller that is responsible for views that may show notifications counter
    */
    public class BaseController : Controller
    {
        protected readonly INotificationInterface _notificationService;

        public BaseController(INotificationInterface notificationService)
        {
            _notificationService = notificationService;
        }

        //method that gets executed when controller is used to trigger unseen notification counter update and show it on inherited pages
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userId = HttpContext.Session.GetInt32("UserId"); 
            if (userId != null)
            {
                var unseenCount = _notificationService.GetUnseenNotificationsAsync(userId.Value).Result.Count;
                HttpContext.Session.SetInt32("UnseenNotificationsCount", unseenCount);
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
