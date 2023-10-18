namespace MyFilmBevy.Services.Interfaces
{
    public interface IImagService
    {
        Task<byte[]> EncodeImageAsync(IFormFile file);
        Task<byte[]> EncodeImageAsync(string fileName);
        Task<byte[]> EncodeImageUrlAsyc(string fileName);

        string DecodeImage(byte[] data, string type);
        string ImageType(IFormFile file);
        int FileSize(IFormFile file);
    }
}
