using Microsoft.AspNetCore.Http;
using SkiaSharp;

namespace ShipRate.Application.Utilities
{
    public interface IImageUtility
    {
        Task<byte[]> CompressImageAsync(IFormFile file, CancellationToken cancellationToken);
    }

    public class ImageUtility : IImageUtility
    {
        private const int MaxDimension = 1920;
        private const int Quality = 75;

        public async Task<byte[]> CompressImageAsync(IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
                return Array.Empty<byte>();

            // خواندن استریم فایل به حافظه
            await using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream, cancellationToken);
            memoryStream.Position = 0;

            // بارگذاری تصویر به همراه متادیتا
            using var codec = SKCodec.Create(memoryStream);
            if (codec == null)
                throw new InvalidOperationException("قالب تصویر پشتیبانی نمی‌شود یا فایل نامعتبر است.");

            // اصلاح جهت چرخش بر اساس EXIF (مانند AutoOrient در ImageSharp)
            var origin = codec.EncodedOrigin;
            using var originalBitmap = SKBitmap.Decode(codec);
            using var orientedBitmap = FixOrientation(originalBitmap, origin);

            // محاسبه ابعاد جدید با حفظ نسبت (ResizeMode.Max)
            int targetWidth = orientedBitmap.Width;
            int targetHeight = orientedBitmap.Height;

            if (targetWidth > MaxDimension || targetHeight > MaxDimension)
            {
                float ratio = Math.Min((float)MaxDimension / targetWidth, (float)MaxDimension / targetHeight);
                targetWidth = (int)(targetWidth * ratio);
                targetHeight = (int)(targetHeight * ratio);
            }

            var imageInfo = new SKImageInfo(targetWidth, targetHeight);

            // ریسایز تصویر با کیفیت بالا (High Sinc sampling)
            using var resizedBitmap = orientedBitmap.Resize(imageInfo, SKSamplingOptions.Default);
            using var image = SKImage.FromBitmap(resizedBitmap ?? orientedBitmap);

            // خروجی نهایی به صورت WebP با کیفیت ۷۵
            using var data = image.Encode(SKEncodedImageFormat.Webp, Quality);

            return data.ToArray();
        }

        /// <summary>
        /// اصلاح زاویه تصویر بر اساس تگ Orientation فایل EXIF
        /// </summary>
        private static SKBitmap FixOrientation(SKBitmap bitmap, SKEncodedOrigin origin)
        {
            if (origin == SKEncodedOrigin.TopLeft)
                return bitmap;

            using var image = SKImage.FromBitmap(bitmap);
            SKBitmap rotated;

            switch (origin)
            {
                case SKEncodedOrigin.BottomRight: // 180 degrees
                    rotated = new SKBitmap(bitmap.Width, bitmap.Height);
                    using (var canvas = new SKCanvas(rotated))
                    {
                        canvas.RotateDegrees(180, bitmap.Width / 2f, bitmap.Height / 2f);
                        canvas.DrawImage(image, 0, 0, SKSamplingOptions.Default);
                    }
                    bitmap.Dispose();
                    return rotated;

                case SKEncodedOrigin.RightTop: // 90 CW
                    rotated = new SKBitmap(bitmap.Height, bitmap.Width);
                    using (var canvas = new SKCanvas(rotated))
                    {
                        canvas.Translate(bitmap.Height, 0);
                        canvas.RotateDegrees(90);
                        canvas.DrawImage(image, 0, 0, SKSamplingOptions.Default);
                    }
                    bitmap.Dispose();
                    return rotated;

                case SKEncodedOrigin.LeftBottom: // 270 CW (90 CCW)
                    rotated = new SKBitmap(bitmap.Height, bitmap.Width);
                    using (var canvas = new SKCanvas(rotated))
                    {
                        canvas.Translate(0, bitmap.Width);
                        canvas.RotateDegrees(270);
                        canvas.DrawImage(image, 0, 0, SKSamplingOptions.Default);
                    }
                    bitmap.Dispose();
                    return rotated;

                default:
                    return bitmap;
            }
        }
    }
}
