using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandleInTheWind.API.Models.Vouchers
{
    public class VoucherDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime Expired { get; set; }

        public double Value { get; set; }

        public int Points { get; set; }
    }
}
