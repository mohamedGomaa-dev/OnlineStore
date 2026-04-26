using AutoMapper;
using FluentAssertions;
using Moq;
using Store.DataAccess.Helpers;
using Store.DataAccess.Units.implementations;
using Store.DataAccess.Units.interfaces;
using Store.Models.Entities;
using Store.Services.Dtos.ReviewDtos;
using Store.Services.Helpers;
using Store.Services.Services.implementations;
using Store.Services.Services.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Store.Services.Tests.Services
{
    public class ReviewServiceTest
    {

        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly IReviewService _reviewService;

        public ReviewServiceTest()
        {
            // SUTS: System Under Test which is the service we want to test
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _reviewService = new ReviewService(_unitOfWorkMock.Object, _mapperMock.Object);
        }
        [Fact]
        public async Task AddReviewAsync_WhenUserDoesNotExist_ReturnsFailure()
        {
            // Arrange
            var reviewCreateDto = new ReviewCreateDto
            {
                UserId = 1,
                ProductId = 1,
                Rating = 5,
                ReviewText = "Great product!"
            };
            _unitOfWorkMock.Setup(u => u.Users.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(false);
            // Act
            // Act
            var result = await _reviewService.AddReviewAsync(reviewCreateDto);
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal($"user with id: {reviewCreateDto.UserId} not found", result.Message);
        }

        [Fact]
        public async Task AddReviewAsync_WhenProductIdDoesNotExist_ReturnsFailure()
        {
            // Arrange
            var reviewCreateDto = new ReviewCreateDto
            {
                UserId = 1,
                ProductId = 11,
                Rating = 5,
                ReviewText = "Great product!"
            };
            _unitOfWorkMock.Setup(u => u.Users.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(u => u.Products.ExistsAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(false);
            // Act
            var result = await _reviewService.AddReviewAsync(reviewCreateDto);
            // Assert
            Assert.False(result.IsSuccess);
            result.Message.Should().Contain($"product with id: {reviewCreateDto.ProductId} not found");
        }

        [Fact]
        public async Task AddReviewAsync_WhenUserReviewedProductAlready_ReturnsFailure()
        {
            var reviewCreateDto = new ReviewCreateDto
            {
                UserId = 1,
                ProductId = 11,
                Rating = 5,
                ReviewText = "Great product!"
            };
            _unitOfWorkMock.Setup(u => u.Users.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(u => u.Products.ExistsAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(u => u.Reviews.ExistsAsync(It.IsAny<Expression<Func<Review, bool>>>())).ReturnsAsync(true);
            // Act
            var result = await _reviewService.AddReviewAsync(reviewCreateDto);
            // Assert
            Assert.False(result.IsSuccess);
            result.Message.Should().Contain("user reviewed this product already!");
        }

        [Fact]
        public async Task AddReviewAsync_WhenUserHasnotBoughtProduct_ReturnsFailure()
        {
            var reviewCreateDto = new ReviewCreateDto
            {
                UserId = 1,
                ProductId = 11,
                Rating = 5,
                ReviewText = "Great product!"
            };
            _unitOfWorkMock.Setup(u => u.Users.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(u => u.Products.ExistsAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(u => u.Reviews.ExistsAsync(It.IsAny<Expression<Func<Review, bool>>>())).ReturnsAsync(false);
            _unitOfWorkMock.Setup(u => u.Orders.ExistsAsync(It.IsAny<Expression<Func<Order, bool>>>())).ReturnsAsync(false);
            // Act
            var result = await _reviewService.AddReviewAsync(reviewCreateDto);
            // Assert
            Assert.False(result.IsSuccess);
            result.Message.Should().Contain("User must buy the product first to review it.");
        }
        [Fact]
        public async Task AddReviewAsync_WhenUserHasBoughtProduct_ReturnsSuccess()
        {
            // Arrange
            var reviewCreateDto = new ReviewCreateDto
            {
                UserId = 1,
                ProductId = 11,
                Rating = 5,
                ReviewText = "Great product!"
            };

            var reviewEntity = new Review { UserId = 1, ProductId = 11 };
            var expectedDto = new ReviewDto { UserId = 1, ProductId = 11 };

            // Setup Mocks for validation checks
            _unitOfWorkMock.Setup(u => u.Users.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(true);
            _unitOfWorkMock.Setup(u => u.Products.ExistsAsync(It.IsAny<Expression<Func<Product, bool>>>())).ReturnsAsync(true);
            _unitOfWorkMock.Setup(u => u.Reviews.ExistsAsync(It.IsAny<Expression<Func<Review, bool>>>())).ReturnsAsync(false);
            _unitOfWorkMock.Setup(u => u.Orders.ExistsAsync(It.IsAny<Expression<Func<Order, bool>>>())).ReturnsAsync(true);

            // FIX: Setup Mapper to return an object instead of null
            _mapperMock.Setup(m => m.Map<Review>(It.IsAny<ReviewCreateDto>())).Returns(reviewEntity);
            _mapperMock.Setup(m => m.Map<ReviewDto>(It.IsAny<Review>())).Returns(expectedDto);

            // Act
            var result = await _reviewService.AddReviewAsync(reviewCreateDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("successfully added review", result.Message);

            // Verify that the review was actually added to the repository
            _unitOfWorkMock.Verify(u => u.Reviews.AddAsync(It.IsAny<Review>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitChanges(), Times.Once);

        }

        [Fact]
        public async Task GetProductReviewsAsync_WhenProductDoesntExist_ReturnsFaliure()
        {
            // Arrange 
            int productId = 1;

            _unitOfWorkMock.Setup(u => u.Products.ExistsAsync(It.IsAny<Expression<Func<Product, bool>>>())).ReturnsAsync(false);
            // Act

            var result = await _reviewService.GetProductReviewsAsync(productId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain($"product with id: {productId} not found");
        }

        [Fact]
        public async Task GetProductReviewsAsync_WhenProductExists_ReturnsSuccess()
        {
            // Arrange 
            int productId = 1;
            var reviewDtos = new List<ReviewDto>();
            var reviews = new List<Review>();
            _unitOfWorkMock.Setup(u => u.Products.ExistsAsync(It.IsAny<Expression<Func<Product, bool>>>())).ReturnsAsync(true);
            _unitOfWorkMock.Setup(u => u.Reviews.GetAllAsync(It.IsAny<Expression<Func<Review, bool>>>())).ReturnsAsync(reviews);
            

            var result = await _reviewService.GetProductReviewsAsync(productId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Contain("Success");
        }

        [Fact] async Task GetProductReviewsAsync_WhenWeUseReviewQuery_ReturnsSuccess()
        {

        }
        

    }
}
