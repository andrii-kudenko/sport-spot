/*
    Author: Nhat Truong Luu, Omar
    Description: Controller Class of all Method related to Event such as CRUD of Event database or Searching Event Functionality
 */

using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SportSpot.Entities.Models;
using SportSpot.Services.Interfaces;

namespace SportSpot.Operations.Controllers
{
    public class EventController : Controller
    {
        // Event Service using Sqlite Service
        private readonly IEventInterface _eventInterface;

        // User Service using Sqlite Service
        private readonly IUserInterface _userInterface;

        // Constructor to inject the Interface for Event and User
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

        // Method to return View of Search page for Event
        public async Task<IActionResult> Search()
        {
            return View();
        }

        /* 
            Author: Omar
            Method with HTTP GET for Searching Event
            Parameter: string location: Location of Event
                       Sports sportType: The Sport Type of Event
            Return: Partial Event if Event Result
         */

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

        /* 
            Author: Omar
            Method with for showing Event Details
            Parameter: int id: Id of Event
            Return: View of Event or Index View if Error
         */
        public async Task<IActionResult> EventDetails(int id)
        {
            // Get user in this session
            var userId = HttpContext.Session.GetInt32("UserId");
            var user = await _userInterface.GetUserByIdAsync((int)userId);

            try
            {
                var @event = await _eventInterface.GetEventByIdAsync(id);
                if (@event == null)
                    return NotFound();

                ViewBag.CurrentUserId = userId;

                return View(@event);
            }
            catch (Exception ex)
            {
                // Log the error
                return RedirectToAction(nameof(Index));
            }
        }

        /* 
            Author: Omar
            Method with HTTP GET for Creating Event
         */
        [HttpGet]
        public IActionResult CreateEvent()
        {
            return View(new Event
            {
                Date = DateTime.Now,
                RequiredPlayers = 1
            });
        }

        /* 
            Author: Omar
            Method with HTTP POST for Creating Event
            Parameter: Event @event: Event model from View
            Return: Views according to Validity of ModelState
         */
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
                        return RedirectToAction("Login", "Auth"); // Go to Login page for authentication
                    }

                    @event.CreatorId = userId.Value; // Assign User Id to the Creator Id
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


        /* 
            Author: Omar
            Method For Index Event
         */
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

        /* 
            Author: Nhat Truong Luu
            Method with HTTP GET for Editting Event
            Parameter: int id: Id of event
            Return: Event View according to the Event
         */
        [HttpGet]
        public async Task<IActionResult> EditEvent(int id)
        {

            //if (user.Id != initialEvent.CreatorId)
            //{

            //    return RedirectToAction(nameof(Index));
            //};

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

        /* 
            Author: Nhat Truong Luu
            Method with HTTP POST for Editting Event
            Parameter: Event event: Event model from View
            Return: Event View according to the ModelState
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEvent(Event @event)
        {
            try
            {
                // Check if Mode is Valid or now
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

        /* 
            Author: Omar
            Method with HTTP POST for Deleting Event
            Parameter: int id: Id of Event
            Return: Index View
         */
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

        /* 
            Author: Nhat Truong, Omar
            Method with HTTP POST for User joining Event
            Parameter: int eventId: Id of Event
            Return: Event Views
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JoinEvent(int eventId)
        {
            try
            {
                // Get Event chosen
                var @event = await _eventInterface.GetEventByIdAsync(eventId);
                if (@event == null)
                    return NotFound();

                // Get the current user based on Session
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