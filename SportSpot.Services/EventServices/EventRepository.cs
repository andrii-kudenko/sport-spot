using System;
using SportSpot.Entities;

namespace SportSpot.Services.EventServices
{
	public class EventRepository : IEventServices
	{
		public readonly EventDbContext _context;

        public EventRepository(EventDbContext context)
        {
            _context = context;
        }

        // Get all Events
        public List<Event> GetEvents()
        {
            return _context.Events.ToList();
        }

        // Get Event based on Id
        public Event GetEvent(int id)
        {
            return _context.Events.FirstOrDefault(e => e.Id == id);
        }

        // Add event
        public Event AddEvent(Event e)
        {
            _context.Events.Add(e);
            _context.SaveChanges();
            return e;
        }

        // Delete event
        public string DeleteEvent(Event e)
        {
            _context.Events.Remove(e);
            _context.SaveChanges();
            return "Delete Successfully";
        }

        // Update Event
        public Event UpdateEvent(Event e)
        {
            _context.Events.Update(e);
            _context.SaveChanges();
            return e;
        }

        // Get Event by Sports

        // Get Event by Postal Code

        // Get Event by City
    }
}

