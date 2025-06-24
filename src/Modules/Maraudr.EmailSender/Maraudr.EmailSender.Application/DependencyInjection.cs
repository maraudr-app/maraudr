using Maraudr.EmailSender.Application.UseCases;
using Maraudr.EmailSender.Application.UseCases.SendWelcomeEmail;
using Microsoft.Extensions.DependencyInjection;

namespace Maraudr.EmailSender.Application
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ISendWelcomeEmailHandler, SendWelcomeEmailHandler>();
            services.AddScoped<ISendResetLinkEmailHandler, SendResetLinkEmailHandler>();

        }
    }
}
