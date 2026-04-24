//using FluentAssertions;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.IdentityModel.Tokens;
//using Moq;
//using Store.API.Controllers;
//using Store.Services.Dtos.OrderDtos;
//using Store.Services.Helpers;
//using Store.Services.Services.interfaces;
//using System.Security.Claims;
//using Utility = Store.Services.Helpers.Utility;

//namespace Store.API.Tests
//{
//    public class OrdersControllerTests
//    {
//        private readonly Mock<IOrderService> _mockOrderService;
//        private readonly OrdersController _controller;

//        public OrdersControllerTests()
//        {
//            _mockOrderService = new Mock<IOrderService>();
//            _controller = new OrdersController(_mockOrderService.Object);
//        }

//        // --- SENIOR DEV HELPER METHOD ---
//        // Call this in your Arrange phase to simulate a logged-in user!
//        private void SimulateLoggedInUser(int userId, string role = "User")
//        {
//            var claims = new List<Claim>
//            {
//                // Note: Ensure this matches exactly how your GetUserId() extension method reads the ID!
//                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
//                new Claim(ClaimTypes.Role, role)
//            };
//            var identity = new ClaimsIdentity(claims, "TestAuthType");
//            var claimsPrincipal = new ClaimsPrincipal(identity);

//            _controller.ControllerContext = new ControllerContext
//            {
//                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
//            };
//        }
//        // --------------------------------

//        [Fact]
//        public async Task GetOrderById_WhenOrderDoesNotExist_ReturnsNotFound()
//        {
//            // Arrange
//            int orderId = 99;
//            // TODO: Setup _mockOrderService to return Utility.Failure<OrderDto?>("not found")
//            _mockOrderService.Setup(o => o.GetOrderByIdAsync(orderId)).ReturnsAsync(Utility.Failure<OrderDto?>("not found"));
//            // Act
//            // TODO: Call the controller method
//            var result = _controller.GetOrderById(orderId);
//            // Assert
//            // TODO: Assert that the result is of type NotFoundObjectResult
//            result.Should().Be(StatusCodes.Status404NotFound);
//        }

//        [Fact]
//        public async Task GetOrderById_WhenUserIsNotOwnerAndNotAdmin_ReturnsForbidden()
//        {
//            // Arrange
//            int orderId = 1;
//            int orderOwnerId = 10;
//            int loggedInUserId = 5; // Different user!

//            var fakeOrder = new OrderDto { Id = orderId, UserId = orderOwnerId };

//            // TODO: Setup _mockOrderService to return Success with the fakeOrder
//            // TODO: Call SimulateLoggedInUser(loggedInUserId)

//            // Act
//            // TODO: Call the controller method

//            // Assert
//            // TODO: Assert that the result is an ObjectResult with a StatusCode of 403
//        }

//        [Fact]
//        public async Task GetOrderById_WhenUserIsOwner_ReturnsOk()
//        {
//            // Arrange
//            int orderId = 1;
//            int userId = 10;

//            var fakeOrder = new OrderDto { Id = orderId, UserId = userId };

//            // TODO: Setup _mockOrderService to return Success with the fakeOrder
//            // TODO: Call SimulateLoggedInUser(userId) // The logged in user matches the owner!

//            // Act
//            // TODO: Call the controller method

//            // Assert
//            // TODO: Assert that the result is of type OkObjectResult
//            // Optional bonus: Assert that the OkObjectResult.Value is equivalent to your fakeOrder
//        }
//    }
//}