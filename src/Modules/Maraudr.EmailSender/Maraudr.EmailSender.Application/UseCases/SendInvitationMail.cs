using System;
using System.Threading.Tasks;
using Maraudr.EmailSender.Application.Dtos;
using Maraudr.EmailSender.Domain.Interfaces;

namespace Maraudr.EmailSender.Application.UseCases;
public interface ISendInvitationMailHandler
{
    public Task HandleAsync(SendInvitationRequest request);
}
public class SendInvitationMailHandler(IMailService service):ISendInvitationMailHandler
{
    public async Task HandleAsync(SendInvitationRequest request)
    {
        var invitationLink = $"http://localhost:3000/accept-invitation?token={Uri.EscapeDataString(request.Token)}";
          await service.SendEmailAsync(new MailRequest
            {
                ToEmail = request.InvitedEmail,
                Subject = $"Invitation à rejoindre {request.AssociationName}",
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
                    .message-box {{ background-color: #f9f9f9; border-left: 4px solid #3b82f6; padding: 15px; margin: 15px 0; }}
                </style>
            </head>
            <body>
                <div class='header'>
                    <h1>Invitation à rejoindre {request.AssociationName}</h1>
                </div>
                <div class='content'>
                    <p>Bonjour,</p>

                    <p>{request.InvitingUser} vous invite à rejoindre <strong>{request.AssociationName}</strong>, une plateforme de gestion d'associations et d'événements.</p>

                    {(string.IsNullOrEmpty(request.Message) ? "" : $@"
                    <div class='message-box'>
                        <p><strong>Message de {request.InvitingUser} :</strong></p>
                        <p>{request.Message}</p>
                    </div>")}

                    <div class='section'>
                        <h3>Avec Maraudr, vous pourrez :</h3>
                        <ul>
                            <li>Créer et rejoindre des associations</li>
                            <li>Participer à des événements</li>
                            <li>Gérer votre planning personnel</li>
                            <li>Rester connecté avec votre communauté</li>
                        </ul>
                    </div>

                    <div class='section'>
                        <h3>Pour accepter cette invitation :</h3>
                        <a href='{invitationLink}' class='button'>Créer mon compte</a>
                        <p>Ce lien d'invitation expirera dans 7 jours.</p>
                    </div>

                    <p>Si vous avez des questions, n'hésitez pas à contacter la personne qui vous a invité ou notre équipe de support.</p>

                    <p>À très bientôt sur Maraudr !</p>
                    <p>L'équipe Maraudr</p>
                </div>
                <div class='footer'>
                    <p>© 2025 Maraudr. Tous droits réservés.</p>
                    <p>Cet email a été envoyé à {request.InvitedEmail}</p>
                </div>
            </body>
            </html>"
            });
    }
}