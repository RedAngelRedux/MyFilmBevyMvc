using MyFilmBevy.Services.Interfaces;

namespace MyFilmBevy.Services
{
    public class BasicImageService : IImageService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BasicImageService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public string? DecodeImage(byte[] data, string type)
        {
            if (data is null || type is null) return null;

            return $"data:image/{type};base64,{Convert.ToBase64String(data)}";
        }

        public async Task<byte[]?> EncodeImageAsync(IFormFile file)
        {
            if (file is null) return null;

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            return ms.ToArray();
        }

        public async Task<byte[]?> EncodeImageAsync(string fileName)
        {
            var file = $"{Directory.GetCurrentDirectory()}/wwwroot/images/{fileName}";
            return await File.ReadAllBytesAsync(file);
        }

        public async Task<byte[]?> EncodeImageUrlAsync(string imageUrl)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(imageUrl);
            using Stream stream = await response.Content.ReadAsStreamAsync();
            var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            return ms.ToArray();
        }

        public int FileSize(IFormFile file)
        {
            return Convert.ToInt32(file?.Length);
        }

        public string? ImageType(IFormFile file)
        {
            return file?.ContentType;
        }
    }
}
