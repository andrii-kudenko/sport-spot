using Microsoft.AspNetCore.Mvc;
using SportSpot.Entities.Models;
using SportSpot.Operations.Models;
using SportSpot.Services.Interfaces;

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

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Events(string? category, string? searchTerm, DateOnly? date, string? location)
        {
            var selectedCategory = !string.IsNullOrEmpty(category)
                ? Enum.Parse<Sports>(category)
                : (Sports?)null;

            var filteredEvents = await _eventInterface.GetFilteredEvents(searchTerm, selectedCategory, date, location);

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

        /*[HttpGet]
        public async Task<IActionResult> SearchEvents(string location, Sports? sportType)
        {
            List<Event> events = new();

            if (!string.IsNullOrEmpty(location))
            {
                events = await _eventInterface.SearchEventsByLocationAsync(location);
            }
            else if (sportType.HasValue)
            {
                events = await _eventInterface.SearchEventsBySportTypeAsync(sportType.Value);
            }

            return PartialView("_EventSearchResults", events);
        }*/

        [HttpGet]
        public async Task<IActionResult> SearchUsers(string city, Sports? sport)
        {
            List<User> users = new();

            if (!string.IsNullOrEmpty(city))
            {
                users = await _userInterface.SearchUsersByCityAsync(city);
            }
            else if (sport.HasValue)
            {
                users = await _userInterface.SearchUsersBySportAsync(sport.Value);
            }

            return PartialView("_UserSearchResults", users);
        }
    }
}
