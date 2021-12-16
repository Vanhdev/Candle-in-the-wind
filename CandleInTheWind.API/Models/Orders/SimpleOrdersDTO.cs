﻿using CandleInTheWind.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandleInTheWind.API.Models.Orders
{
    public class SimpleOrderDTO
    {
        public int Id { get; set; }
        public decimal Total { get; set; }
        public OrderStatus Status { get; set; }

        public string StatusName { get; set; }

        public IEnumerable<string> ProductName { get; set; }

        public DateTime PurchaseDate { get; set; }


        /*
        private string toStatusName(OrderStatus status)
        {
            switch (status)
            {
                case OrderStatus.Pending : return "Đang xử lý";
                case OrderStatus.Approved : return "Đã duyệt";
                case OrderStatus.Canceled : return "Đã huỷ";
                case OrderStatus.NotApproved : return "Không được duyệt";
                    default : return "";
            }
        } */
    }

    public static class getEnumNames
    {
        public static string toStatusName(this OrderStatus status)
        {
            switch (status)
            {
                case OrderStatus.Pending: return "Đang xử lý";
                case OrderStatus.Approved: return "Đã duyệt";
                case OrderStatus.Canceled: return "Đã huỷ";
                case OrderStatus.NotApproved: return "Không được duyệt";
                default: return "";
            }
        }
    }

    
}