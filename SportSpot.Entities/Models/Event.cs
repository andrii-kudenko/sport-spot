using System;
using System.ComponentModel.DataAnnotations;

namespace SportSpot.Entities.Models
{
    public class Event
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Location { get; set; }

        public Sports SportType { get; set; }

        [Range(1, 100)]
        public int RequiredPlayers { get; set; }

        //place holder not required bc we need to make account
        public virtual List<User>? RegisteredPlayers { get; set; }
        public virtual User? Creator { get; set; }  
        public int CreatorId { get; set; }
    }
}

