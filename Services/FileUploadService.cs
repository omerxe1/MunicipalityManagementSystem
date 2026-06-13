using System.Security.Cryptography;
using System.Text;

namespace kocaaliv2.Services
{
    /// <summary>
    /// Güvenli dosya upload servisi
    /// Sadece izin verilen dosya türleri, boyut kontrolü ve güvenli dosya adı
    /// </summary>
    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string[] _allowedExtensions = { ".pdf", ".jpg", ".jpeg", ".png" };
        private readonly string[] _allowedMimeTypes = { 
            "application/pdf", 
            "image/jpeg", 
            "image/jpg", 
            "image/png" 
        };

        public FileUploadService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        /// <summary>
        /// Dosyayı güvenli şekilde yükler
        /// </summary>
        public async Task<(bool Success, string? FileName, string? ErrorMessage)> UploadFileAsync(
            IFormFile file, 
            string uploadFolder, 
            long maxSizeInBytes = 5242880)
        {
            // Dosya null kontrolü
            if (file == null || file.Length == 0)
            {
                return (false, null, "Dosya seçilmedi veya boş.");
            }

            // Dosya boyutu kontrolü
            if (file.Length > maxSizeInBytes)
            {
                return (false, null, $"Dosya boyutu {maxSizeInBytes / 1024 / 1024}MB'dan büyük olamaz.");
            }

            // Dosya uzantısı kontrolü
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
            {
                return (false, null, "Sadece PDF, JPG, JPEG ve PNG dosyaları yüklenebilir.");
            }

            // MIME type kontrolü
            if (!_allowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
            {
                return (false, null, "Geçersiz dosya türü.");
            }

            try
            {
                // Güvenli dosya adı oluştur (random + timestamp)
                var safeFileName = GenerateSafeFileName(extension);

                // Upload klasörünü wwwroot içinde oluştur
                var uploadPath = Path.Combine(_environment.WebRootPath, uploadFolder);
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Dosya yolunu kontrol et (wwwroot dışına çıkış engelle)
                var fullPath = Path.Combine(uploadPath, safeFileName);
                var fullPathNormalized = Path.GetFullPath(fullPath);
                var webRootNormalized = Path.GetFullPath(_environment.WebRootPath);

                if (!fullPathNormalized.StartsWith(webRootNormalized))
                {
                    return (false, null, "Güvenlik hatası: Geçersiz dosya yolu.");
                }

                // Dosyayı kaydet
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Relative path döndür
                var relativePath = Path.Combine(uploadFolder, safeFileName).Replace("\\", "/");
                return (true, relativePath, null);
            }
            catch (Exception ex)
            {
                return (false, null, $"Dosya yükleme hatası: {ex.Message}");
            }
        }

        /// <summary>
        /// Dosyayı siler
        /// </summary>
        public Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                var fullPath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/'));
                var normalizedPath = Path.GetFullPath(fullPath);
                var webRootNormalized = Path.GetFullPath(_environment.WebRootPath);

                // wwwroot dışına çıkış engelle
                if (!normalizedPath.StartsWith(webRootNormalized))
                {
                    return Task.FromResult(false);
                }

                if (System.IO.File.Exists(normalizedPath))
                {
                    System.IO.File.Delete(normalizedPath);
                    return Task.FromResult(true);
                }

                return Task.FromResult(false);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// Güvenli, random dosya adı oluşturur
        /// </summary>
        private string GenerateSafeFileName(string extension)
        {
            var timestamp = DateTime.Now.Ticks;
            var randomBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            var randomString = Convert.ToBase64String(randomBytes)
                .Replace("+", "")
                .Replace("/", "")
                .Replace("=", "")
                .Substring(0, 16);

            return $"{timestamp}_{randomString}{extension}";
        }
    }
}

