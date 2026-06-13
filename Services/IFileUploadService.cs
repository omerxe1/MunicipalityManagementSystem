namespace kocaaliv2.Services
{
    /// <summary>
    /// Güvenli dosya upload servisi interface'i
    /// </summary>
    public interface IFileUploadService
    {
        /// <summary>
        /// Dosyayı güvenli şekilde yükler
        /// </summary>
        Task<(bool Success, string? FileName, string? ErrorMessage)> UploadFileAsync(
            IFormFile file, 
            string uploadFolder, 
            long maxSizeInBytes = 5242880); // Default 5MB

        /// <summary>
        /// Dosyayı siler
        /// </summary>
        Task<bool> DeleteFileAsync(string filePath);
    }
}






