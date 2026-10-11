using Microsoft.AspNetCore.Http;

namespace ShipRate.Application.Utilities
{
    public static class PictureHelper
    {
        public static async Task<byte[]?> ConvertToBytesAsync(IFormFile? file)
        {
            if (file == null || file.Length == 0)
                return null;

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            return ms.ToArray();
        }
    }
}
