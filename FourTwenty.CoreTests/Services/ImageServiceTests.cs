using System;
using System.IO;
using System.Threading.Tasks;
using FourTwenty.Core.Interfaces;
using FourTwenty.Core.Services;
using ImageMagick;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FourTwenty.CoreTests.Services
{
    [TestClass]
    public class ImageServiceTests
    {
        private class HostingEnvironment : IWebHostEnvironment
        {
            public string EnvironmentName { get; set; }
            public string ApplicationName { get; set; }
            public string WebRootPath { get; set; } = Path.GetTempPath();
            public IFileProvider WebRootFileProvider { get; set; }
            public string ContentRootPath { get; set; }
            public IFileProvider ContentRootFileProvider { get; set; }
        }

        private readonly IImageService _imageService;

        private const string MockImage =
            @"/9j/2wCEAAgGBgcGBQgHBwcJCQgKDBQNDAsLDBkSEw8UHRofHh0aHBwgJC4nICIsIxwcKDcpLDAxNDQ0Hyc5PTgyPC4zNDIBCQkJDAsMGA0NGDIhHCEyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMv/AABEIAZABkAMBIgACEQEDEQH/xAGiAAABBQEBAQEBAQAAAAAAAAAAAQIDBAUGBwgJCgsQAAIBAwMCBAMFBQQEAAABfQECAwAEEQUSITFBBhNRYQcicRQygZGhCCNCscEVUtHwJDNicoIJChYXGBkaJSYnKCkqNDU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6g4SFhoeIiYqSk5SVlpeYmZqio6Slpqeoqaqys7S1tre4ubrCw8TFxsfIycrS09TV1tfY2drh4uPk5ebn6Onq8fLz9PX29/j5+gEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoLEQACAQIEBAMEBwUEBAABAncAAQIDEQQFITEGEkFRB2FxEyIygQgUQpGhscEJIzNS8BVictEKFiQ04SXxFxgZGiYnKCkqNTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqCg4SFhoeIiYqSk5SVlpeYmZqio6Slpqeoqaqys7S1tre4ubrCw8TFxsfIycrS09TV1tfY2dri4+Tl5ufo6ery8/T19vf4+fr/2gAMAwEAAhEDEQA/APAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigB+z60bPrTqKAG7PrRs+tOooAbs+tGz606igBuz60bPrTqKAG7PrRs+tOooAbs+tGz606igBuz60bPrTqKAG7PrRs+tOooAbs+tGz606igBuz60bPrTqKAG7PrRs+tOooAbs+tGz606igBuz60bPrTqKAG7PrRs+tOooAbs+tGz606igBuz60bPrTqKAG7PrRs+tOooAbs+tGz606igBuz60bPrTqKAG7PrRs+tOooAbs+tGz606igBuz60bPrTqKAG7PrRs+tOooAbs+tGz606igBuz60bPrTqKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigBQrMCVUnAycelJW94KUyeNtFi8pZhNeRRPGwyHRmCsp9ipI/Guz+Kvwsm8I3LappEck2iSnkKCxtT2DHup7H8D2yAeXUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRXuPwm+EA1FIvEPiSA/ZThrSybjzOfvOP7voO/f3ALHwW+GF1Fe2/ivWYfKjVS1jC33iSOJCOwxnAPrmvebq0gvLWS3uYklhlUo8bruVgRggjuOalVQFxgAelOoA+Vvin8Kp/CVxJq2lq02iyuSRjm1JPCn1Xng/ge2fL6+8by0t721ktrqGOaCRSskcigqy+hB7V8o/FbwLaeEfERTSpxLazxm4NvnL2ylto3f7JJwCee3PUgH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APvhTkUpOBk14J8IPi20zxeHPEt5lztSyu5MDOBjy3b16YPfoTmuv+MvifU9H0Cw0rRmZdQ1mc2yuo+YLjB2+hJZRntk45xQBhfEP4q3s+qN4S8FRyT6rI5t5biNclG7rH7jnLHgYPpkcfF4E+MNmV1OK8vvPX59h1MM5PXlSxB+nNex/D34ead4K0hMRJJqsyD7Xc9SW7qvooJ/Hqa7XAxQB8JapJLLql1JPALedpWMsQXaEfPIwenOeO1VK+qPi58NIfFOkvqmmWqjW7dcrt+Xz0HJU+ren5V8rkEHBGD70AFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFAAK9G0n4hTaneeFE1+XzX0TUEkjuXPLwsVDBvUrtBBPUZ74z5zRQB98AgjNLXgnwf+LbSvbeGfEM6DCiOyu3JyxzgRuf0B9sda953fL+FAEV3dW9lay3V1KkVvEheSRzhVUDJJP0r4W1CZLjUrqaIYjkmd0HsSSK9Z+MXxRl1q7uPDWjyldMhfZcyr1uHB5X/cB/PHpXjtABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFe7fCz4svJp8fhXWJG+1Mvk6deMeCxyFSQk+pAB/PpmvCaAcHNAD5VdZXEoO8HDZ6570ynyyNNK0jks7HJY9SfU0ygAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAopu4UbhQA6im7hRuFADqKbuFG4UAOopu4UbhQA6im7hRuFADqKbuFG4UAOopu4UbhQA6im7hRuFADqKbuFG4UAOopu4UbhQA6im7hRuFADqKbuFG4UAOopu4UbhQA6im7hRuFADqKbuFG4UAOopu4UbhQA6im7hRuFADqKbuFG4UAOopu4UbhQA6im7hRuFADqKbuFG4UAOopu4UbhQA6im7hRuFADqKbuFG4UAOopu4UbhQAyiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooA//9k=";

        public ImageServiceTests()
        {
            _imageService = new ImageService(new HostingEnvironment());
        }

        #region thumnails test
        [TestMethod]
        public async Task ThumbnailTest()
        {
            var randomSize = new Random().Next(5, 399);
            var filePath = GetTempFileName();
            var resultFileName = await _imageService.CreateThumbnail(randomSize, randomSize, filePath, "_mythumbnail");
            var resultFilePath = Path.Combine(Path.GetTempPath(), resultFileName);
            using (var imageMagick = new MagickImage(resultFilePath))
            {
                Assert.AreEqual(randomSize, imageMagick.Width);
            }
            Assert.IsTrue(Path.GetFileNameWithoutExtension(resultFileName).EndsWith("_mythumbnail"));
            File.Delete(resultFilePath);
            File.Delete(filePath);
        } 
        #endregion

        #region compression test

        [TestMethod]
        public void CompressLosslessTest()
        {
            CompressTestLocal(true);
        }

        [TestMethod]
        public void CompressTest()
        {
            CompressTestLocal(false);
        }

        private void CompressTestLocal(bool lossless)
        {
            var file = GetTempFileName();
            var oldFileSize = new FileInfo(file).Length;
            var result = _imageService.CompressImage(file, lossless);
            Assert.AreEqual(true, result);
            var newFileSize = new FileInfo(file).Length;
            Assert.IsTrue(newFileSize < oldFileSize);
            File.Delete(file);
        }
        #endregion

        #region format tests
        [TestMethod]
        public async Task WebPFormatTest()
        {
            string extension = ".webp";
            var result = await _imageService.ConvertToFormat(GetTempFileName(), MagickFormat.WebP, extension);
            CheckFormatResult(result, MagickFormat.WebP, extension);
        }


        [TestMethod]
        public async Task Jpeg2000FormatTest()
        {
            string extension = ".jp2";
            var result = await _imageService.ConvertToFormat(GetTempFileName(), MagickFormat.Jp2, extension);
            CheckFormatResult(result, MagickFormat.Jp2, extension);
        }


        private void CheckFormatResult(string filePath, MagickFormat expectedFormat, string expectedExtension)
        {
            using (var imageMagick = new MagickImage(filePath))
            {
                Assert.AreEqual(expectedFormat, imageMagick.Format);
            }
            if (File.Exists(filePath))
                File.Delete(filePath);
            Assert.AreEqual(Path.GetExtension(filePath), expectedExtension);
        }
        #endregion

        #region private methods


        private string GetTempFileName()
        {
            var data = Convert.FromBase64String(MockImage);
            var tempFileName = Path.GetTempFileName();
            File.WriteAllBytes(tempFileName, data);
            return tempFileName;
        }

        #endregion
    }
}
