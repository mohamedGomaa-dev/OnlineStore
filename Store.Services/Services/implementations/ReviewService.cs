using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Store.DataAccess.Helpers;
using Store.DataAccess.Units.interfaces;
using Store.Models.Entities;
using Store.Services.Dtos.ReviewDtos;
using Store.Services.Helpers;
using Store.Services.Services.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Services.Services.implementations
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<Result<ReviewDto>> AddReviewAsync(ReviewCreateDto dto)
        {
            // check if the user id exists
            // check if the product id exists
            // check if the user has bought the product before so the review would be honest
            // check if the user reviewed this product before
            // create the review with a datetime.utcnow and commit changes

            if (!await _unitOfWork.Users.ExistsAsync(u => u.Id == dto.UserId))
            {
                return Utility.Failure<ReviewDto>($"user with id: {dto.UserId} not found");
            }
            if (!await _unitOfWork.Products.ExistsAsync(u => u.Id == dto.ProductId))
            {
                return Utility.Failure<ReviewDto>($"product with id: {dto.ProductId} not found");
            }
            if (await _unitOfWork.Reviews.ExistsAsync(r => r.UserId ==  dto.UserId && r.ProductId == dto.ProductId))
            {
                return Utility.Failure<ReviewDto>($"user reviewed this product already!");
            }

            bool hasBoughtProduct = await _unitOfWork.Orders.ExistsAsync(o =>
    o.UserId == dto.UserId &&
    o.OrderItems.Any(i => i.ProductId == dto.ProductId));

            if (!hasBoughtProduct)
            {
                return Utility.Failure<ReviewDto>("User must buy the product first to review it.");
            }
            var review = _mapper.Map<Review>(dto);
            review.ReviewDate = DateTime.UtcNow;
            await _unitOfWork.Reviews.AddAsync(review);
            await _unitOfWork.CommitChanges();
            _cache.Remove("reviews");
            return Utility.Success<ReviewDto>("successfully added review", _mapper.Map<ReviewDto>(review));

        }

        public async Task<Result<IEnumerable<ReviewDto>>> GetProductReviewsAsync(int productId)
        {
            if (!await _unitOfWork.Products.ExistsAsync(p => p.Id  == productId))
            {
                return Utility.Failure<IEnumerable<ReviewDto>>($"product with id: {productId} not found");

            }
            var reviews = await _unitOfWork.Reviews.GetAllAsync(r => r.ProductId == productId);

            return Utility.Success<IEnumerable<ReviewDto>>("Success", _mapper.Map<IEnumerable<ReviewDto>>(reviews));
        }

        public async Task<PagedResult<IEnumerable<ReviewDto>>> GetProductReviewsAsync(ReviewQuery query)
        {
            var result = await _unitOfWork.Reviews.GetReviewsAsync(query);
            var reviews = _mapper.Map<IEnumerable<ReviewDto>>(result.Items);
            return Utility.SuccessPaged(reviews, result.TotalCount, query.PageNumber, query.PageSize);
        }
    }
}
