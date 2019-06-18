using System.Security.Claims;

namespace FourTwenty.Core.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal principal) =>
            principal.FindFirstValue(ClaimTypes.NameIdentifier);
        public static string GetPicture(this ClaimsPrincipal principal, string pictureClaimName = "Picture") =>
            principal.FindFirstValue(pictureClaimName);
    }
}
