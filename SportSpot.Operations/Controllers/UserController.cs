using System;
using Microsoft.AspNetCore.Mvc;
using SportSpot.Entities.Models;
using SportSpot.Services.Interfaces;

namespace SportSpot.Operations.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserInterface _userInterface;
        private readonly IEventInterface _eventInterface;

        public UserController(IUserInterface userInterface, IEventInterface eventInterface)
        {
            _userInterface = userInterface;
            _eventInterface = eventInterface;

        }

        

        public async Task<IActionResult> Profile(int id)
        {
            var user = await _userInterface.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            var events = await _eventInterface.GetEventsByCreatorAsync(id);
            ViewBag.IsLoggedInUser = HttpContext.Session.GetInt32("UserId") == id;
            ViewBag.UserEvents = events;

            return View(user);
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
    }
}

