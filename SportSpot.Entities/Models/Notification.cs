using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSpot.Entities.Models
{
    /*
        Author: Dan Chystov
        Description of class: Data class for notification functioonality
    */
    public class Notification
    {
        //notification id
        public int Id { get; set; }

        //id of the user who receives notification
        public int UserId { get; set; } 

        //message showed in notification
        public string Message { get; set; }

        //url that holds controller name to react to notification
        public string ActionUrl { get; set; } 

        //is notification seen
        public bool IsSeen { get; set; } = false;

        //when notification was created
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
