using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandleInTheWind.Models
{
    public class OrderProduct 
    {
        [Required]
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

        public int? ProductId { get; set; }
        public virtual Product Product { get; set; }

        [Required]
        [Column(TypeName = "money")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
