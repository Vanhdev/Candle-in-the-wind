using CandleInTheWind.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace CandleInTheWind.API.Models.Users
{
    public class UserDTO
    {
        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [Required]
        public Gender Gender { get; set; }

    }
}
