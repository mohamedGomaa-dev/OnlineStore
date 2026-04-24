using Store.DataAccess.Helpers;
using Store.Services.Dtos.ReviewDtos;
using Store.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Services.Services.interfaces
{
    public interface IReviewService
    {
        Task<Result<ReviewDto>> AddReviewAsync(ReviewCreateDto dto);
        Task<Result<IEnumerable<ReviewDto>>> GetProductReviewsAsync(int productId);
        Task<PagedResult<IEnumerable<ReviewDto>>> GetProductReviewsAsync(ReviewQuery query);
    }
}
