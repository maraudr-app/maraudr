using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Maraudr.EmailSender.Application.Dtos;
using Maraudr.EmailSender.Domain.Interfaces;

namespace Maraudr.EmailSender.Application.UseCases.SendWelcomeEmail
{
    internal class SendWelcomeEmailHandler(IMailService mailService) : ISendWelcomeEmailHandler
    {
        public async Task HandleAsync(MailToQuery query)
        {
            await mailService.SendEmailAsync(new MailRequest
            {
                ToEmail = query.ToEmail,
                Subject = "Welcome to Maraudr",
                Body = $"Hello {query.Name},\nWelcome to Maraudr! We are excited to have you on board.\nBest regards,\nMaraudr Team"
                                
            });
        }
    }
}
