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
    Subject = "Bienvenue sur Maraudr !",
    Body = $@"
<!DOCTYPE html>
    <html>
    <head>
        <style>
            body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; }}
            .header {{ background: linear-gradient(135deg, #f97316 0%, #3b82f6 100%); color: white; padding: 20px; text-align: center; }}
            .content {{ padding: 20px; }}
            .footer {{ background-color: #f2f2f2; padding: 15px; text-align: center; font-size: 12px; }}
            .button {{ display: inline-block; background: linear-gradient(135deg, #f97316 0%, #3b82f6 100%); color: white; padding: 10px 20px; 
                      text-decoration: none; border-radius: 5px; margin: 20px 0; }}
            .section {{ margin-bottom: 20px; }}
        </style>
    </head>
    <body>
        <div class='header'>
            <h1>Bienvenue sur Maraudr !</h1>
        </div>
        <div class='content'>
            <p>Bonjour {query.Name},</p>
            
            <p>Nous sommes ravis de vous accueillir sur <strong>Maraudr</strong>, votre nouvelle plateforme de gestion d'associations et d'événements.</p>
            
            <div class='section'>
                <h3>Avec votre compte, vous pouvez maintenant :</h3>
                <ul>
                    <li>Créer et rejoindre des associations</li>
                    <li>Organiser et participer à des événements</li>
                    <li>Gérer votre planning personnel</li>
                    <li>Rester connecté avec votre communauté</li>
                </ul>
            </div>
            
            <div class='section'>
                <h3>Prêt à commencer ?</h3>
                <p>Complétez votre profil et rejoignez votre première association dès aujourd'hui !</p>
                <a href='https://maraudr-front-737l.onrender.com' class='button'>Accéder à mon compte</a>
            </div>
            
            
            <p>À très bientôt sur Maraudr !</p>
            <p>L'équipe Maraudr</p>
        </div>
        <div class='footer'>
            <p>© 2025 Maraudr. Tous droits réservés.</p>
            <p>Cet email a été envoyé à {query.ToEmail}</p>
        </div>
    </body>    
    </html>"
});
        }
    }
}
