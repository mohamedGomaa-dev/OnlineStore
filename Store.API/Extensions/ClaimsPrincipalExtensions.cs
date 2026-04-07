using System.Security.Claims;

namespace Store.API.Extensions
{
    // a class to get the claims that we need to authorize our controllers
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : 0;
        }

        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            return user.IsInRole("Admin");
        }
    }
}
