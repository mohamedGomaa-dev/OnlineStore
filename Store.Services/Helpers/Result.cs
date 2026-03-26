using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Services.Helpers
{
    public class Result
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class Result<T> : Result
    {
        public T? Data { get; set; }
    }
    public class PagedResult<T> : Result<T>
    {
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages
        {
            get
            {

                return Convert.ToInt32(Math.Ceiling(Convert.ToDouble(TotalCount)/Convert.ToDouble(PageSize)));
            }
        }
    }
}
