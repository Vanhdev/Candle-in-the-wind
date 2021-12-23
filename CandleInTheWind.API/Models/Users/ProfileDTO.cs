using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandleInTheWind.API.Models.Users
{
    public class ProfileDTO : UserDTO
    {
        public string Email { get; set; }

        public int Points { get; set; }

        public string GenderName { get; set; }
    }
}
