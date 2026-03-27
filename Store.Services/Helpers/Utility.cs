using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Services.Helpers
{
    public class Utility
    {
        public static Result Failure(string message)
        {
            return new Result()
            {
                IsSuccess = false,
                Message = message
            };
        }

        public static Result Success(string message)
        {
            return new Result()
            {
                IsSuccess = true,
                Message = message
            };
        }

        public static Result<T> Success<T>(string message, T data)
        {
            return new Result<T>()
            {
                IsSuccess = true,
                Message = message,
                Data = data
            };
        }

        public static Result<T> Failure<T>(string message)
        {
            return new Result<T>()
            {
                IsSuccess = false,
                Message = message,
            };
        }
        public static PagedResult<T> SuccessPaged<T>( T data, int totalCount, int pageNumber, int pageSize,
            string message = "")
        {
            return new PagedResult<T>()
            {
                IsSuccess = true,
                Message = message,
                Data = data,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
