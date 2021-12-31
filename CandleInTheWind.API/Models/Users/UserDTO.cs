using CandleInTheWind.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
