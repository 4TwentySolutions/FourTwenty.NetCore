using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace FourTwenty.Core.Extensions
{
    public static class Extensions
    {
        /// <summary>
        /// fix for default temp folder limit (https://docs.microsoft.com/ru-ru/dotnet/api/system.io.path.gettempfilename?view=netcore-2.2)
        /// </summary>
        /// <param name="env">Current website environment</param>
        /// <param name="folderName">Newly created temporary folder name</param>
        [Obsolete]
        public static void FixTemporaryFolder(this IHostingEnvironment env, string folderName = "TEMP")
        {
            #region 
            var tempPath = Path.Combine(env.WebRootPath, folderName);
            if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);

            Environment.SetEnvironmentVariable("TEMP", tempPath);
            Environment.SetEnvironmentVariable("TMP", tempPath);
            #endregion

        }

#if NETCOREAPP3_1
        /// <summary>
        /// fix for default temp folder limit (https://docs.microsoft.com/ru-ru/dotnet/api/system.io.path.gettempfilename?view=netcore-2.2)
        /// </summary>
        /// <param name="env">Current website environment</param>
        /// <param name="folderName">Newly created temporary folder name</param>
        public static void FixTemporaryFolder(this IWebHostEnvironment env, string folderName = "TEMP")
        {
        #region 
            var tempPath = Path.Combine(env.WebRootPath, folderName);
            if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);

            Environment.SetEnvironmentVariable("TEMP", tempPath);
            Environment.SetEnvironmentVariable("TMP", tempPath);
        #endregion

        }
#endif

    }
}
