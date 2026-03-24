using Shared.Data.DTOs.EmailDTOs;

namespace EmailNotification.WebHook.Repositories.Services
{
    public interface IEmailService
    {
        public string SendEmail(EmailDto emailDto);
    }
}
