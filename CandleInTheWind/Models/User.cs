using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CandleInTheWind.Models
{
    public enum Gender
    {
        Female = 0,
        Male,
        Other
    }

    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [Required]
        [StringLength(256)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [StringLength(20)]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string PasswordHash { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Required]
        [EnumDataType(typeof(Gender))]
        [Column(TypeName = "tinyint")]
        public Gender Gender { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Points { get; set; } = 0;

        public virtual ICollection<Order> Orders{ get; set; }
        public virtual ICollection<Cart> Carts { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
