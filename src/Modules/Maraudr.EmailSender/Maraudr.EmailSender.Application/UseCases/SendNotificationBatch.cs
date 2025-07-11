using Maraudr.EmailSender.Application.Dtos;
using Maraudr.EmailSender.Domain.Interfaces;

namespace Maraudr.EmailSender.Application.UseCases;

public interface ISendNotificationBatch
{
    public Task HandleAsync(SendNotificationBatchQuery query);
}

public class SendNotificationBatch(IMailService mailService) : ISendNotificationBatch
{
    public async Task HandleAsync(SendNotificationBatchQuery query)
    {
        foreach (var data in query.userData)
        {
            mailService.SendEmailAsync(new MailRequest
            {
                ToEmail = data,
                Subject = $"Invitation à participer à {query.eventTitle}",
                Body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; }}
        .header {{ background: linear-gradient(135deg, #f97316 0%, #3b82f6 100%); color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; }}
        .footer {{ background-color: #f2f2f2; padding: 15px; text-align: center; font-size: 12px; color: #777; }}
        .button {{ display: inline-block; background: linear-gradient(135deg, #f97316 0%, #3b82f6 100%); color: white; padding: 10px 20px; 
                  text-decoration: none; border-radius: 5px; margin: 20px 0; }}
        .section {{ margin-bottom: 20px; }}
    </style>
</head>
<body>
    <div class='header'>
        <h1>Invitation à un événement</h1>
    </div>
    <div class='content'>
        <p>Bonjour,</p>

        <p>Tu es invité(e) à participer à l'événement suivant :</p>

        <div class='section'>
            <h2>{query.eventTitle}</h2>
            <p>{query.eventDescription}</p>
        </div>

        <p>Pour plus de détails et confirmer ta participation, connecte-toi à ton espace personnel :</p>
        <a href='https://maraudr.eu/mon-compte' class='button'>Accéder à mon compte</a>

        <p>Si tu as des questions, n'hésite pas à visiter notre <a href='https://maraudr.eu/aide'>centre d'aide</a> ou contacter notre équipe de support.</p>

        <p>Merci et à bientôt sur Maraudr !<br>L'équipe Maraudr</p>
    </div>
    <div class='footer'>
        <p>© 2025 Maraudr. Tous droits réservés.</p>
        <p>Cet email a été envoyé à {data}</p>
    </div>
</body>
</html>"
            });
        }
    }
}
    
