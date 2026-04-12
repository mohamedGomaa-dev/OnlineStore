using AutoMapper;
using FluentAssertions;
using Moq;
using Store.DataAccess.Units.implementations;
using Store.DataAccess.Units.interfaces;
using Store.Models.Entities;
using Store.Services.Dtos.OrderDtos;
using Store.Services.Helpers;
using Store.Services.Services.implementations;
using Store.Services.Services.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Store.Tests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfwork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly IOrderService _orderService;
        public OrderServiceTests()
        {
            // SUTs : system under tests
            _mockUnitOfwork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _orderService = new OrderService(_mockUnitOfwork.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task CreateOrderAsync_WhenOrderItemsAreNull_ReturnsFaliure()
        {
            // Arrange
            var dto = new OrderCreateDto
            {
                Items = null
            };
            
            // Act
            var result = await _orderService.CreateOrderAsync(dto);
            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("empty");
        }
        [Fact]
        public async Task CreateOrderAsync_WhenUserDoesntExist_ReturnsFaliure()
        {
            // Arrange
            var dto = new OrderCreateDto
            {
                Items = new List<OrderItemCreateDto> { new OrderItemCreateDto { ProductId = 1, Quantity = 55 } }
            };
            _mockUnitOfwork.Setup(u => u.Users.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(false);
            // Act
            var result = await _orderService.CreateOrderAsync(dto);
            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("user");
        }
        [Fact]
        public async Task CreateOrderAsync_WhenProductIsNull_ReturnsFaliure()
        {
            // Arrange
            var dto = new OrderCreateDto
            {
                Items = new List<OrderItemCreateDto> { new OrderItemCreateDto { ProductId = 1, Quantity = 55 } }
            };
            _mockUnitOfwork.Setup(u => u.Users.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(true);
            _mockUnitOfwork.Setup(u => u.Products.ExistsAsync(It.IsAny<Expression<Func<Product, bool>>>()))
               .ReturnsAsync(false);
            // Act
            var result = await _orderService.CreateOrderAsync(dto);
            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("product");
            result.Message.Should().Contain("not found");
        }
        [Fact]
        public async Task CreateOrderAsync_WhenQuantityExceedsStock_ShouldReturnFailure()
        {
            // ARRANGE
            var dto = new OrderCreateDto
            {
                UserId = 1,
                Items = new List<OrderItemCreateDto> { new OrderItemCreateDto { ProductId = 1, Quantity = 55 } }
            };
            _mockUnitOfwork.Setup(u => u.Users.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>()))
                           .ReturnsAsync(true);

            var fakeProduct = new Product { Id = 1, QuantityStock = 2 };
            _mockUnitOfwork.Setup(u => u.Products.GetAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                           .ReturnsAsync(fakeProduct);
            _mockUnitOfwork.Setup(u => u.Products.ExistsAsync(It.IsAny<Expression<Func<Product, bool>>>())).ReturnsAsync(true);
            // ACT
            var result = await _orderService.CreateOrderAsync(dto);
            // ASSERT
            result.IsSuccess.Should().BeFalse();
            // should contian stock to assert that the product doesn't have enough stock
            result.Message.ToLower().Should().Contain("stock");
        }

        [Fact]
        public async Task CreateOrderAsync_WhenQuantityDoesntExceedStock_ShouldReturnFailure()
        {
            // ARRANGE
            var dto = new OrderCreateDto
            {
                UserId = 1,
                Items = new List<OrderItemCreateDto> { new OrderItemCreateDto { ProductId = 1, Quantity = 1 } }
            };
            _mockUnitOfwork.Setup(u => u.Users.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>()))
                           .ReturnsAsync(true);

            var fakeProduct = new Product { Id = 1, QuantityStock = 2 };
            _mockUnitOfwork.Setup(u => u.Products.GetAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                           .ReturnsAsync(fakeProduct);
            _mockUnitOfwork.Setup(u => u.Products.ExistsAsync(It.IsAny<Expression<Func<Product, bool>>>())).ReturnsAsync(true);
            var mappedOrder = new Order { UserId = 1 };
            _mockMapper.Setup(m => m.Map<Order>(It.IsAny<OrderCreateDto>()))
                       .Returns(mappedOrder);
            _mockUnitOfwork.Setup(u => u.Orders.AddAsync(mappedOrder));
            // ACT
            var result = await _orderService.CreateOrderAsync(dto);
            // ASSERT
            result.IsSuccess.Should().BeTrue();
            _mockUnitOfwork.Verify(u => u.CommitChanges(), Times.Once);
            // should contian stock to assert that the product doesn't have enough stock
            result.Message.ToLower().Should().Contain("successfully");
        }

        [Fact]
        public async Task GetOrderByIdAsync_WhenIdEqualToZero_ShouldReturnFaliure()
        {
            // Arrange
            int id = 0;
            
            // Act
            var result = await _orderService.GetOrderByIdAsync(id);
            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("valid id");
        }
        [Fact]
        public async Task GetOrderByIdAsync_WhenIdLessThanZero_ShouldReturnFaliure()
        {
            // Arrange
            int id = -1;

            // Act
            var result = await _orderService.GetOrderByIdAsync(id);
            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("valid id");
        }

        [Fact]
        public async Task GetOrderByIdAsync_WhenOrderIsNotFound_ShouldReturnFaliure()
        {
            // Arrange
            int id = 19;
            Order fakeOrder = null;
            _mockUnitOfwork.Setup(u => u.Orders.GetOrderByIdWithItemsAsync(id))
                                   .ReturnsAsync(fakeOrder);
            // Act
            var result = await _orderService.GetOrderByIdAsync(id);
            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("not found");
        }
        [Fact]
        public async Task GetOrderByIdAsync_WhenOrderIsFound_ShouldReturnSuccess()
        {
            // Arrange
            int id = 19;
            Order fakeOrder = new Order { Id = id };
            _mockUnitOfwork.Setup(u => u.Orders.GetOrderByIdWithItemsAsync(id))
                                   .ReturnsAsync(fakeOrder);
            // Act
            var result = await _orderService.GetOrderByIdAsync(id);
            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Contain("found order!");
        }
       


    }
}
