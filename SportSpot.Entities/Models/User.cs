using System;
using System.ComponentModel.DataAnnotations;

namespace SportSpot.Entities.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Range(13, 100)]
        public int Age { get; set; }

        [Range(0, 300)]
        public int Height { get; set; }

        [Range(0, 500)]
        public int Weight { get; set; }

        [Phone]
        public string? Phone { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }  

        public Sports Sports { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string City { get; set; }

        public string? Address { get; set; }

        public string? PostalCode { get; set; }
    }
}

