namespace Maraudr.EmailSender.Application.Dtos;

public class SendInvitationRequest
{
     public string InvitingUser{
          get;
          set;
     }
     public string AssociationName{
          get;
          set;
     }
     public string InvitedEmail{
          get;
          set;
     }
     public string Token{
          get;
          set;
     }
     public string Message{
          get;
          set;
     }
}