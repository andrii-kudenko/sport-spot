using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SportSpot.Entities.Models;
using SportSpot.Operations.Models;
using SportSpot.Services.Interfaces;
using SportSpot.Services.Services;

namespace SportSpot.Operations.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserInterface _userInterface;
        private readonly IEventInterface _eventInterface;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(IUserInterface userInterface, IEventInterface eventInterface, INotificationInterface notificationService): base(notificationService)
        {
            
            _userInterface = userInterface;
            _eventInterface = eventInterface;
        }

        

        public async Task<IActionResult> Profile(int id)
        {

            var loggedInUserId = HttpContext.Session.GetInt32("UserId");
            if (loggedInUserId == null)
            {
                return RedirectToAction("Login", "Auth"); 
            }

            var user = await _userInterface.GetUserByIdAsync(id); 
            if (user == null) return NotFound(); 

            // Fetch events created by the profile user
            var events = await _eventInterface.GetEventsByCreatorAsync(id);

            var model = new ProfileViewModel
            {
                ProfileUser = user,
                LoggedInUserId = loggedInUserId.Value, // Value is non-null due to the earlier check
                Events = events,
                Friends = await _userInterface.GetFriendsAsync(loggedInUserId.Value),
                FriendRequests = await _userInterface.GetUsersByIdsAsync(user.FriendRequests)
            };

            return View(model); // Pass the view model to the view
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile(int id)
        {
            var user = await _userInterface.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(User user)
        {
            if (ModelState.IsValid)
            {
                await _userInterface.UpdateUserAsync(user);
                return RedirectToAction(nameof(Profile), new { id = user.Id });
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> SendFriendRequest(int targetUserId)
        {
            var loggedInUserId = HttpContext.Session.GetInt32("UserId");
            if (loggedInUserId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            await _userInterface.SendFriendRequestAsync(loggedInUserId.Value, targetUserId);
            await _notificationService.AddNotificationAsync(targetUserId, "You have a new friend request!", "/User/FriendRequests");
            return RedirectToAction("Profile", new {id = targetUserId});
        }

        [HttpPost]
        public async Task<IActionResult> AcceptFriendRequest(int requesterId)
        {
            var loggedInUserId = HttpContext.Session.GetInt32("UserId");
            if (loggedInUserId == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            await _userInterface.AcceptFriendRequestAsync(loggedInUserId.Value, requesterId);
            await _notificationService.AddNotificationAsync(requesterId,"Your friend request has been accepted!","/User/Friends");
            return RedirectToAction("FriendRequests");
        }

        [HttpPost]
        public async Task<IActionResult> DeclineFriendRequest(int requesterId)
        {
            var loggedInUserId = HttpContext.Session.GetInt32("UserId");
            if (loggedInUserId == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            await _userInterface.DeclineFriendRequestAsync(loggedInUserId.Value, requesterId);
            await _notificationService.AddNotificationAsync(requesterId,"Your friend request was declined.","/User/Search");

            return RedirectToAction("FriendRequests");
        }

        //sending list of users who requested to view
        [HttpGet]
        public async Task<IActionResult> ViewFriendRequests()
        {
            var loggedInUserId = HttpContext.Session.GetInt32("UserId");
            if (loggedInUserId == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            var user = await _userInterface.GetUserByIdAsync(loggedInUserId.Value);
            if (user == null)
            {
                return NotFound();
            }

            var friendRequests = await _userInterface.GetUsersByIdsAsync(user.FriendRequests);
            return View(friendRequests);
        }

        [HttpGet]
        public async Task<IActionResult> Friends()
        {
            var loggedInUserId = HttpContext.Session.GetInt32("UserId");
            if (loggedInUserId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var friends = await _userInterface.GetFriendsAsync(loggedInUserId.Value);
            return View(friends);
        }


        [HttpGet]
        public async Task<IActionResult> FriendRequests()
        {
            var loggedInUserId = HttpContext.Session.GetInt32("UserId");
            if (loggedInUserId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var pendingRequests = await _userInterface.GetPendingFriendRequestsAsync(loggedInUserId.Value);
            return View(pendingRequests);
        }

        [HttpGet]
        public async Task<IActionResult> Invintations()
        {
            var loggedInUserId = HttpContext.Session.GetInt32("UserId");
            if (loggedInUserId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var pendingRequests = await _userInterface.GetPendingInvintationAsync(loggedInUserId.Value);
            return View(pendingRequests);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFriend(int friendId)
        {
            var loggedInUserId = HttpContext.Session.GetInt32("UserId");
            if (loggedInUserId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            await _userInterface.RemoveFriendAsync(loggedInUserId.Value, friendId);

            return RedirectToAction("Friends");
        }
        [HttpPost]
        public async Task<IActionResult> AcceptInvintation(int eventId)
        {
            var loggedInUserId = HttpContext.Session.GetInt32("UserId");
            if (loggedInUserId == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            await _userInterface.AcceptInvintationAsync(loggedInUserId.Value, eventId);
            await _notificationService.AddNotificationAsync(eventId, "Your friend request has been accepted!", "/User/Friends");
            return RedirectToAction("Profile", new { id = loggedInUserId});
        }

        [HttpPost]
        public async Task<IActionResult> DeclineInvintation(int eventId)
        {
            var loggedInUserId = HttpContext.Session.GetInt32("UserId");
            if (loggedInUserId == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            await _userInterface.DeclineInvintationAsync(loggedInUserId.Value, eventId);
            await _notificationService.AddNotificationAsync(eventId, "Your friend request was declined.", "/User/Search");

            return RedirectToAction("Profile", new { id = loggedInUserId });
        }
    }
}

