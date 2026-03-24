using EmailNotification.WebHook.Repositories.Services;
using MimeKit;
using Shared.Data.DTOs.EmailDTOs;

namespace EmailNotification.WebHook.Repositories.Implementations
{
    public class EmailServiceImplementation : IEmailService
    {
        public string SendEmail(EmailDto emailDto)
        {
            bool isEmailSent = false;

            MimeMessage email = new();

            email.From.Add(MailboxAddress.Parse(""));
            email.To.Add(MailboxAddress.Parse(""));
            email.Subject = emailDto.Title;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = emailDto.Content
            };

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.Connect(host: "smtp.gmail.com", port: 587, options: MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate(userName: "", password: "", cancellationToken: CancellationToken.None);
            smtp.Send(message: email);
            isEmailSent = true;
            smtp.Disconnect(quit: true);

            return isEmailSent ? "Email sent successfully" : "Email sending failed";
        }
    }
}
