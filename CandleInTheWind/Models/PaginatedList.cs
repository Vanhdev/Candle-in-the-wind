using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CandleInTheWind.Models
{
    public enum Status
    {
        Success, Fail, None
    }
    public static class Constants
    {
        public const int PageSize = 3;
    }
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            this.AddRange(items);
        }
        public PaginatedList(T item, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            this.Add(item);
        }
        public bool PreviousPage
        {
            get { return PageIndex > 1; }
        }
        public bool NextPage
        {
            get { return PageIndex < TotalPages; }
        }
        public static async Task<PaginatedList<T>> GetListAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
        public static PaginatedList<T> GetList(T source, int pageIndex, int pageSize)
        {
            return new PaginatedList<T>(source, 1, pageIndex, pageSize);
        }
    }
}