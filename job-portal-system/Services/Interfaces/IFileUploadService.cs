namespace job_portal_system.Services.Interfaces
{
    public interface IFileUploadService
    {
        Task<string?> UploadImageAsync(IFormFile file, string folder);
        Task<string?> UploadFileAsync(IFormFile file, string folder);
        Task<bool> DeleteFileAsync(string filePath);
    }
}
