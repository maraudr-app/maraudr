
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Maraudr.EmailSender.Domain.Interfaces;
using Maraudr.EmailSender.Application.Dtos;
using Maraudr.EmailSender.Endpoints.MailSettings;
using Microsoft.Extensions.Options;

namespace Maraudr.EmailSender.Infrastructure;

    internal class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }
  
        public async Task SendEmailAsync(MailRequest mailRequest)
        {

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_mailSettings.Mail));
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;
            var builder = new BodyBuilder();

            builder.HtmlBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();
            
            Console.WriteLine("mail " + _mailSettings.Mail);
            Console.WriteLine("password " + _mailSettings.Password);
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
}
