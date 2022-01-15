using System.ComponentModel.DataAnnotations;

namespace CandleInTheWind.API.Models.Accounts
{
    public class SignInDTO
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
