using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Maraudr.EmailSender.Application.UseCases.SendWelcomeEmail;
using Microsoft.Extensions.DependencyInjection;

namespace Maraudr.EmailSender.Application
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ISendWelcomeEmailHandler, SendWelcomeEmailHandler>();
      

        }
    }
}
