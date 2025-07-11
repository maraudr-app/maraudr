using System;
using System.Threading.Tasks;
using Maraudr.EmailSender.Application.Dtos;
using Maraudr.EmailSender.Domain.Interfaces;

namespace Maraudr.EmailSender.Application.UseCases;

public interface ISendResetLinkEmailHandler
{
    public Task HandleAsync(ResetPasswordMailRequest query);

}


public class SendResetLinkEmailHandler(IMailService mailService) : ISendResetLinkEmailHandler
{
    public async Task HandleAsync(ResetPasswordMailRequest query)
    {
        try
        {
            var resetLink = $"https://www.maraudr.eu/reset-password?token={Uri.EscapeDataString(query.Token)}";
            await mailService.SendEmailAsync(new MailRequest
            {
                ToEmail = query.ToEmail,
                Subject = "Réinitialisation de votre mot de passe Maraudr",
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
            <h1>Réinitialisation de mot de passe</h1>
        </div>
        <div class='content'>
            <p>Bonjour {query.Name},</p>
            
            <p>Nous avons reçu une demande de réinitialisation du mot de passe pour votre compte <strong>Maraudr</strong>.</p>
            
            <div class='section'>
                <h3>Veuillez cliquer sur le bouton ci-dessous pour définir un nouveau mot de passe :</h3>
                <a href='{resetLink}' class='button'>Réinitialiser mon mot de passe</a>
                <p>Ce lien de réinitialisation expirera dans une heure.</p>
            </div>
            
            <p>Si vous n'avez pas demandé de réinitialisation de mot de passe, vous pouvez ignorer cet e-mail en toute sécurité.</p>
            
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
        catch (Exception e)
        {
            Console.WriteLine($"Erreur lors de l'envoi de l'email de réinitialisation: {e.Message}");
            Console.WriteLine($"Détails: {e.StackTrace}");
            throw;
        }
    }
}