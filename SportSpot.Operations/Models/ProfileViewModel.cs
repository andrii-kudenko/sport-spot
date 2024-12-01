using SportSpot.Entities.Models;

namespace SportSpot.Operations.Models
{
    public class ProfileViewModel
    {
        public User ProfileUser { get; set; }  // The user whose profile is being viewed
        public int LoggedInUserId { get; set; } // The ID of the logged-in user
        public List<Event> Events { get; set; } = new List<Event>(); // List of events created by the user
        public List<User> Friends { get; set; } = new List<User>();
        public List<User> FriendRequests { get; set; } = new List<User>();
        public int PendingRequestCount => FriendRequests?.Count ?? 0;
        public int FriendsCount => Friends?.Count ?? 0;
    }
}
