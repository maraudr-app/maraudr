namespace Application.UseCases.Tokens.JwtManagement.GenerateAccessToken;

using Maraudr.User.Domain.Entities.Users; 
using System;
using System.Threading.Tasks;


public interface IGenerateAccessTokenHandler
{
   Task<string> HandleAsync(AbstractUser user);
   Task<DateTime> GetAccessTokenExpirationTime();
}