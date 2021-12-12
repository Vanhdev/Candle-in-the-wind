using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CandleInTheWind.Models
{
    public class Voucher
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Created { get; set; } = DateTime.Now;

        [Required]
        [DataType(DataType.Date)]
        public DateTime Expired { get; set; }

        [Required]
        [Range(0, 100)]
        public double Value { get; set; }
        
        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Points { get; set; } = 0;


        public virtual ICollection<Order> Orders{ get; set; }
    }
}
