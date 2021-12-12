using System;
using System.Collections.Generic;

#nullable disable

namespace CandleInTheWind.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Carts = new HashSet<Cart>();
            Comments = new HashSet<Comment>();
            Orders = new HashSet<Order>();
            Posts = new HashSet<Post>();
        }

        public decimal Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public byte Gender { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Points { get; set; }

        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }
}
