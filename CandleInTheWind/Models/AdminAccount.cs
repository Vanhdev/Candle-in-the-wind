using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CandleInTheWind.Models
{
    public class AdminAccount : IdentityUser
    {
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "Yêu cầu nhập đúng định dạng tài khoản Email!")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}