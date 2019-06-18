using System.Reflection;

namespace FourTwenty.Core.Helpers
{
    public static class AssemblyHelper
    {
        public static string GetCurrentVersion()
        {
            return Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion;
        }
    }
}
