using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSpot.Entities.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; } // The recipient of the notification
        public string Message { get; set; }
        public string ActionUrl { get; set; } // Redirect URL for the notification
        public bool IsSeen { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
