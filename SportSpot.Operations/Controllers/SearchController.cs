using Microsoft.AspNetCore.Mvc;
using SportSpot.Entities.Models;
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
        }

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
