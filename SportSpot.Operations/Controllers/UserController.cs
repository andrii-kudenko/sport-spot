using System;
using Microsoft.AspNetCore.Mvc;
using SportSpot.Entities.Models;
using SportSpot.Services.Interfaces;

namespace SportSpot.Operations.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserInterface _userInterface;

        public UserController(IUserInterface userInterface)
        {
            _userInterface = userInterface;
        }

        public async Task<IActionResult> Profile(int id)
        {
            var user = await _userInterface.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();
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

