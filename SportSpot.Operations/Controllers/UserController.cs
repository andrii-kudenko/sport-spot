using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SportSpot.Entities.Models;
using SportSpot.Operations.Models;
using SportSpot.Services.Interfaces;
using SportSpot.Services.Services;

namespace SportSpot.Operations.Controllers
{
    /*  Author: Omar, Nhat Truong Luu, Danylo Chystov
        Description: Controller for handling notifications
    */
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

        
        //controls profile view, fetches data for user by passed id, sends data to the view
        public async Task<IActionResult> Profile(int id)
        {
            //validate that user is logged in
            var loggedInUserId = HttpContext.Session.GetInt32("UserId");
            if (loggedInUserId == null)
            {
                return RedirectToAction("Login", "Auth"); 
            }

            //get user by passed id
            var user = await _userInterface.GetUserByIdAsync(id); 
            if (user == null) return NotFound(); 

            //fetch events created by the profile user
            var events = await _eventInterface.GetEventsByCreatorAsync(id);

            //use profile model to store information that will be shown on page
            var model = new ProfileViewModel
            {
                ProfileUser = user,
                LoggedInUserId = loggedInUserId.Value, 
                Events = events,
                Friends = await _userInterface.GetFriendsAsync(loggedInUserId.Value),
                FriendRequests = await _userInterface.GetUsersByIdsAsync(user.FriendRequests)
            };

            return View(model); // Pass the view model to the view
        }


        //controls edit profile page, gets user by passd id (currently logged in user), sends data to the view
        [HttpGet]
        public async Task<IActionResult> EditProfile(int id)
        {
            var user = await _userInterface.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();
            return View(user);
        }

        //receives data to update user, updates user on button click
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

        //controls friends page, retrievs friends of the logged in user
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

        //controlls friends request page, fetches request sent to logged in user
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

        //controls remove friend action, triggered on button click
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
    }
}

