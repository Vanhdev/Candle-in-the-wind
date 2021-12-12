﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandleInTheWind.API.Models.Products
{
    public class ProductDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int Stock { get; set; }

        public string ImageUrl { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
