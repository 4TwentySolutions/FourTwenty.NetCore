using System.Threading.Tasks;

namespace FourTwenty.Core.ReCaptcha
{
    public interface IReCaptchaService
    {
        Task<ReCaptchaResponse?> VerifyCaptcha(string token, string secretKey, string? remoteIp = null);
    }
}
