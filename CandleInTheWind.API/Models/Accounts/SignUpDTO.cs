using CandleInTheWind.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace CandleInTheWind.API.Models.Accounts
{
    public class SignUpDTO
    {
        [Required(ErrorMessage = "Tên đăng nhập xác nhận không được để trống")]
        [StringLength(50, ErrorMessage = "Tên đăng nhập không được dài quá 50 kí tự")]
        public string UserName { get; set; }


        [Required(ErrorMessage = "Email xác nhận không được để trống")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }


        [Required(ErrorMessage = "Mật khẩu xác nhận không được để trống")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Required(ErrorMessage = "Mật khẩu xác nhận không được để trống")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; }


        public DateTime? DateOfBirth { get; set; }


        [Required(ErrorMessage = "Giới tính không được để trống")]
        [EnumDataType(typeof(Gender))]
        public Gender Gender { get; set; }
    }
}
