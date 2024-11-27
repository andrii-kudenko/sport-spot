using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SportSpot.Entities.Models;
using SportSpot.Services.Data;
using SportSpot.Services.Interfaces;

namespace SportSpot.Services.Services
{
    public class EventService : IEventInterface
    {
        private readonly SportsDbContext _context;

        public EventService(SportsDbContext context)
        {
            _context = context;
        }

        public async Task<Event> CreateEventAsync(Event @event)
        {
            try
            {
                Console.WriteLine("CreateEventAsync started");
                Console.WriteLine($"Adding event: {JsonSerializer.Serialize(@event)}");

                _context.Events.Add(@event);
                Console.WriteLine("Event added to context");

                await _context.SaveChangesAsync();
                Console.WriteLine("Changes saved to database");

                return @event;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateEventAsync: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<List<Event>> GetAllEventsAsync()
        {
            try
            {
                Console.WriteLine("GetAllEventsAsync started");
                var events = await _context.Events
                    .Include(e => e.Creator)
                    .Include(e => e.RegisteredPlayers)
                    .ToListAsync();
                Console.WriteLine($"Retrieved {events.Count} events");
                return events;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllEventsAsync: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<Event> GetEventByIdAsync(int id)
        {
            return await _context.Events
                .Include(e => e.Creator)
                .Include(e => e.RegisteredPlayers)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        //public async Task<Event> CreateEventAsync(Event @event)
        //{
        //    _context.Events.Add(@event);
        //    await _context.SaveChangesAsync();
        //    return @event;
        //}

        public async Task<Event> UpdateEventAsync(Event @event)
        {
            _context.Entry(@event).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return @event;
        }

        public async Task DeleteEventAsync(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event != null)
            {
                _context.Events.Remove(@event);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Event>> SearchEventsByLocationAsync(string location)
        {
            return await _context.Events
                .Where(e => e.Location.Contains(location))
                .Include(e => e.Creator)
                .Include(e => e.RegisteredPlayers)
                .ToListAsync();
        }

        public async Task<List<Event>> SearchEventsBySportTypeAsync(Sports sportType)
        {
            return await _context.Events
                .Where(e => e.SportType == sportType)
                .Include(e => e.Creator)
                .Include(e => e.RegisteredPlayers)
                .ToListAsync();
        }

        public async Task<List<Event>> GetFilteredEvents(string? searchTerm, Sports? category, DateOnly? date, string? location)
        {
            var events = _context.Events;
            var query = events.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(e => e.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

            if (category.HasValue)
                query = query.Where(e => e.SportType == category);
            if (date.HasValue)
                query = query.Where(e => DateOnly.FromDateTime(e.Date) == date);
            if (!string.IsNullOrEmpty(location))
                query = query.Where(e => e.Location.ToLower().Contains(location.ToLower()));

            /*return await events.ToListAsync();*/
            return await query.ToListAsync();
            return query.ToList();
        }
    }
}

