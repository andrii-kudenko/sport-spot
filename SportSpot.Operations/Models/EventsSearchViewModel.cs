using SportSpot.Entities.Models;

namespace SportSpot.Operations.Models
{
    public class EventsSearchViewModel
    {
        public string? SelectedCategory { get; set; }
        public IEnumerable<string> Categories { get; set; }
        public IEnumerable<Event> SearchResults { get; set; }
        public string? Location { get; set; }
        public DateOnly? Date {  get; set; }
    }
}
