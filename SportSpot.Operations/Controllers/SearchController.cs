/* Author: Andrii Kudenko
   Description: Controller for handling events and users searching
*/
using Microsoft.AspNetCore.Mvc;
using SportSpot.Entities.Models;
using SportSpot.Operations.Models;
using SportSpot.Services.Interfaces;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SportSpot.Operations.Controllers
{
    public class SearchController : Controller
    {
        private readonly IEventInterface _eventInterface;
        private readonly IUserInterface _userInterface;

        public SearchController(IEventInterface eventInterface, IUserInterface userInterface)
        {
            _eventInterface = eventInterface;
            _userInterface = userInterface;
        }

        /*Method for displaying and filtering events based on a set of parameters, like category, date or location of the event*/
        [HttpGet]
        public async Task<IActionResult> Events(string? category, DateOnly? date, string? location)
        {
            var selectedCategory = !string.IsNullOrEmpty(category)
                ? Enum.Parse<Sports>(category)
                : (Sports?)null;

            var filteredEvents = await _eventInterface.GetFilteredEvents(selectedCategory, date, location);

            var categories = Enum.GetNames(typeof(Sports)).ToList();
            
            var viewModel = new EventsSearchViewModel
            {
                SelectedCategory = selectedCategory?.ToString(),
                Categories = categories,
                Date = date,
                Location = location,
                SearchResults = filteredEvents
            };

            return View(viewModel);
        }     
        /*Method for returning a view with the search bar for users*/
        [HttpGet]
        public async Task<IActionResult> Users()
        {
            return View();
        }
        /*Method that handles quering users from the database and returning as json*/
        [HttpGet]
        public async Task<IActionResult> UserResults(string query)
        {
            
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            var results = _userInterface.GetUsersByQuery(query, currentUserId);

            return Json(results);
        }   
    }
}
