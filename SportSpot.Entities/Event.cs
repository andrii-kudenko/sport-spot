using System;
namespace SportSpot.Entities
{
	public class Event
	{
		public int Id { get; set; }

		public string Title { get; set; }

		public string Country { get; set; }

		public string Address { get; set; }

		public string PostalCode { get; set; }

		public DateTime Date { get; set; }

		public int Length { get; set; }

		public Sports Sports { get; set; }

		public int PeopleNeeded { get; set; }
	}
}

