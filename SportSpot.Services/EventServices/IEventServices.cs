using System;
using SportSpot.Entities;

namespace SportSpot.Services.EventServices
{
	public interface IEventServices
	{
		// Get all Event Services
		List<Event> GetEvents();

		// Get a single Event
		Event GetEvent(int id);

        // Add Event
        Event AddEvent(Event e);

        // Delete Event
        string DeleteEvent(Event e);

        // Update Event
        Event UpdateEvent(Event e);
    }
}

