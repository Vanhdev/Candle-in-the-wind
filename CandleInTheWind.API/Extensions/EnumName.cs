using CandleInTheWind.Models;

namespace CandleInTheWind.API.Extensions
{
    public static class EnumName
    {
        public static string GetEnumName(this Gender gender)
        {
            switch (gender)
            {
                case Gender.Female:
                    return "Nữ";
                case Gender.Male:
                    return "Nam";
                case Gender.Other:
                    return "Khác";
                default:
                    return "Khác";
            }
        }

        public static string GetEnumName(this OrderStatus status)
        {
            switch (status)
            {
                case OrderStatus.Pending: 
                    return "Đang xử lý";
                case OrderStatus.Approved: 
                    return "Đã duyệt";
                case OrderStatus.Canceled: 
                    return "Đã huỷ";
                case OrderStatus.NotApproved: 
                    return "Không được duyệt";
                default: 
                    return "";
            }
        }
    }
}
