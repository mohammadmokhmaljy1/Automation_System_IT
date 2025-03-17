using Infrastructure.Core.Interfaces.Application.UtilityServices;
using Microsoft.Extensions.Caching.Memory;

namespace IT_Automation.API.Application.UtilityServices
{
    public class VerificationService : IVerificationService
    {
        private readonly IEmailService _emailService;
        private readonly IMemoryCache _cache;

        public VerificationService(IEmailService emailService, IMemoryCache cache)
        {
            _emailService = emailService;
            _cache = cache;
        }

        public async Task SendVerificationCodeAsync(Guid userId, string email, string subject)
        {
            string verificationCode = GenerateVerificationCode();

            // إرسال البريد الإلكتروني
            await _emailService.SendEmailAsync(email, subject, $"Your verification code is {verificationCode}");

            // تخزين الرمز في Cache )
            _cache.Set($"VerificationCode_{userId}", verificationCode, TimeSpan.FromMinutes(10));
        }
        public async Task SendVerificationCodeAsync(Guid userId, string email, string subject, string token)
        {
            string verificationCode = GenerateVerificationCode();

            // إرسال البريد الإلكتروني
            await _emailService.SendEmailAsync(email, subject, $"Your verification code is {token}");

            // تخزين الرمز في Cache (أو قاعدة البيانات)
            _cache.Set($"VerificationCode_{userId}", verificationCode, TimeSpan.FromMinutes(10));
        }

        public async Task<bool> VerifyCodeAsync(Guid userId, string code)
        {
            string storedCode = _cache.Get<string>($"VerificationCode_{userId}");

            if (storedCode != null && storedCode == code)
            {
                _cache.Remove($"VerificationCode_{userId}");
                return true;
            }

            return false;
        }

        private string GenerateVerificationCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString(); // رمز تحقق من 6 أرقام
        }
    }

}
