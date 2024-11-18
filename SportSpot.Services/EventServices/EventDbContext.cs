using System;
using Microsoft.EntityFrameworkCore;
using SportSpot.Entities;

namespace SportSpot.Services.EventServices
{
	public class EventDbContext : DbContext
	{

		public EventDbContext(DbContextOptions<EventDbContext> options): base(options)
		{

		}

		public DbSet<Event> Events { get; set; }
	}
}

