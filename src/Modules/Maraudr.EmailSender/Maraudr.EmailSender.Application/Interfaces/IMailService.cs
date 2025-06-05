using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Maraudr.EmailSender.Application.Dtos;

namespace Maraudr.EmailSender.Domain.Interfaces
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);

    }
}
