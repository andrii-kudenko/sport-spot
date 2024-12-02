/*
    Author: Omar, Nhat Truong Luu
    Description of class: Data Class of Event
 */
using System;
using System.ComponentModel.DataAnnotations;

namespace SportSpot.Entities.Models
{
    public class Event
    {
        // Field Variable

        // Id of Event
        [Key]
        public int Id { get; set; }


        // Title or name of Event
        [Required]
        [StringLength(100)]
        public string Title { get; set; }


        // Description of Event
        [Required]
        public string Description { get; set; }


        // Date of Event
        [Required]
        public DateTime Date { get; set; }


        // Location of Event
        [Required]
        public string Location { get; set; }


        // Sport Type of Event
        public Sports SportType { get; set; }

        // Number of Players needed
        [Range(1, 100)]
        public int RequiredPlayers { get; set; }

        //place holder not required bc we need to make account
        public virtual List<User>? RegisteredPlayers { get; set; }

        // Navigation Property of the Creator User type
        public virtual User? Creator { get; set; }  

        // Id of the User who created Event
        public int CreatorId { get; set; }
    }
}

