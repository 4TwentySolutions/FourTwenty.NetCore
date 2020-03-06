using System.Security.Claims;

namespace FourTwenty.Core.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal principal) =>
            principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        public static string GetPicture(this ClaimsPrincipal principal, string pictureClaimName = "Picture") =>
            principal.FindFirst(pictureClaimName)?.Value;
    }
}
