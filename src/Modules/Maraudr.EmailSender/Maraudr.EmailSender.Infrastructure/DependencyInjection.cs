using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Maraudr.EmailSender.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Maraudr.EmailSender.Infrastructure
{
  
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IMailService, MailService>();
        }
    }

}
