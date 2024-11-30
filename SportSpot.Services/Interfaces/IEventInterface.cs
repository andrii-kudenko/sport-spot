using System;
using SportSpot.Entities.Models;

namespace SportSpot.Services.Interfaces
{
    public interface IEventInterface
    {
        Task<List<Event>> GetAllEventsAsync();
        Task<Event> GetEventByIdAsync(int id);
        Task<List<Event>> GetEventsByCreatorAsync(int creatorId);
        Task<Event> CreateEventAsync(Event @event);

        Task<Event> UpdateEventAsync(Event @event);
        Task DeleteEventAsync(int id);
        Task<List<Event>> SearchEventsByLocationAsync(string location);
        Task<List<Event>> SearchEventsBySportTypeAsync(Sports sportType);
        Task<List<Event>> GetFilteredEvents(string? searchTerm, Sports? category, DateOnly? date, string? location);
    }
}

