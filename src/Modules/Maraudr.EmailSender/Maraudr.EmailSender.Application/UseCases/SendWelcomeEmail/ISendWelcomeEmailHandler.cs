using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Maraudr.EmailSender.Application.Dtos;

namespace Maraudr.EmailSender.Application.UseCases.SendWelcomeEmail
{
    internal interface ISendWelcomeEmailHandler
    {
        public Task HandleAsync(MailToQuery query);
    }
}
