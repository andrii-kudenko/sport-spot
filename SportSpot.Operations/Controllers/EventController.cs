using System;
using Microsoft.AspNetCore.Mvc;
using SportSpot.Entities.Models;
using SportSpot.Services.Interfaces;

namespace SportSpot.Operations.Controllers
{
    public class EventController : Controller
    {
        private readonly IEventInterface _eventInterface;
        private readonly IUserInterface _userInterface;

        public EventController(IEventInterface eventInterface, IUserInterface userInterface)
        {
            _eventInterface = eventInterface;
            _userInterface = userInterface;
        }

        //public async Task<IActionResult> Index()
        //{
        //    try
        //    {
        //        var events = await _eventInterface.GetAllEventsAsync();
        //        return View(events);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the error
        //        return View(new List<Event>());
        //    }
        //}

        public async Task<IActionResult> Search()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SearchEvents(string location, Sports? sportType)
        {
            try
            {
                var events = new List<Event>();
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
            catch (Exception ex)
            {
                // Log the error
                return PartialView("_EventSearchResults", new List<Event>());
            }
        }

        public async Task<IActionResult> EventDetails(int id)
        {
            try
            {
                var @event = await _eventInterface.GetEventByIdAsync(id);
                if (@event == null)
                    return NotFound();
                return View(@event);
            }
            catch (Exception ex)
            {
                // Log the error
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public IActionResult CreateEvent()
        {
            return View(new Event
            {
                Date = DateTime.Now,
                RequiredPlayers = 1
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(Event @event)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userId = HttpContext.Session.GetInt32("UserId");
                    if (!userId.HasValue)
                    {
                        return RedirectToAction("Login", "Auth");
                    }

                    @event.CreatorId = userId.Value;
                    @event.RegisteredPlayers = new List<User>();

                    await _eventInterface.CreateEventAsync(@event);
                    return RedirectToAction(nameof(Index));
                }
                return View(@event);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the event.");
                return View(@event);
            }
        }


        public async Task<IActionResult> Index()
        {
            try
            {
                Console.WriteLine("Index action started");
                var events = await _eventInterface.GetAllEventsAsync();
                Console.WriteLine($"Retrieved {events.Count()} events");
                return View(events);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Index action: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return View(new List<Event>());
            }
        }
        [HttpGet]
        public async Task<IActionResult> EditEvent(int id)
        {
            try
            {
                var @event = await _eventInterface.GetEventByIdAsync(id);
                if (@event == null)
                    return NotFound();
                return View(@event);
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEvent(Event @event)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _eventInterface.UpdateEventAsync(@event);
                    return RedirectToAction(nameof(EventDetails), new { id = @event.Id });
                }
                return View(@event);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating the event. Please try again.");
                return View(@event);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            try
            {
                await _eventInterface.DeleteEventAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log the error
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JoinEvent(int eventId)
        {
            try
            {
                var @event = await _eventInterface.GetEventByIdAsync(eventId);
                if (@event == null)
                    return NotFound();

                // Get the current user (hardcoded for now)
                var userId = HttpContext.Session.GetInt32("UserId");
                var user = await _userInterface.GetUserByIdAsync((int)userId);
                if (user == null)
                    return NotFound();

                if (@event.RegisteredPlayers == null)
                    @event.RegisteredPlayers = new List<User>();

                if (!@event.RegisteredPlayers.Any(p => p.Id == user.Id))
                {
                    @event.RegisteredPlayers.Add(user);
                    await _eventInterface.UpdateEventAsync(@event);
                }

                return RedirectToAction(nameof(EventDetails), new { id = eventId });
            }
            catch (Exception ex)
            {
                // Log the error
                return RedirectToAction(nameof(EventDetails), new { id = eventId });
            }
        }
    }
}