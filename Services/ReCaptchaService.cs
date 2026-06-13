using System.Text.Json;

namespace kocaaliv2.Services
{
    /// <summary>
    /// Google reCAPTCHA doğrulama servisi
    /// </summary>
    public class ReCaptchaService : IReCaptchaService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly string _secretKey;

        public ReCaptchaService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _secretKey = _configuration["ReCaptcha:SecretKey"] ?? string.Empty;
        }

        /// <summary>
        /// reCAPTCHA token'ını Google API ile doğrular
        /// </summary>
        public async Task<bool> VerifyAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(_secretKey))
                return false;

            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.PostAsync(
                    $"https://www.google.com/recaptcha/api/siteverify?secret={_secretKey}&response={token}",
                    null);

                if (!response.IsSuccessStatusCode)
                    return false;

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ReCaptchaResponse>(jsonResponse);

                return result?.Success == true;
            }
            catch
            {
                return false;
            }
        }

        private class ReCaptchaResponse
        {
            public bool Success { get; set; }
        }
    }
}






