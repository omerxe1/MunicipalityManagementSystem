namespace kocaaliv2.Services
{
    /// <summary>
    /// Google reCAPTCHA doğrulama servisi interface'i
    /// </summary>
    public interface IReCaptchaService
    {
        /// <summary>
        /// reCAPTCHA token'ını doğrular
        /// </summary>
        Task<bool> VerifyAsync(string token);
    }
}






