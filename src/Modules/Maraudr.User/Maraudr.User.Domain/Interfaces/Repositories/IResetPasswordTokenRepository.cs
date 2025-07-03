using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Maraudr.Authentication.Domain.Entities;
using Maraudr.User.Domain.Entities.Tokens;

namespace Maraudr.Authentication.Domain.Interfaces
{
    public interface IResetPasswordTokenRepository
    {
        Task<PasswordResetToken> CreateAsync(PasswordResetToken token);
        Task<PasswordResetToken?> GetByTokenAsync(string token);
        Task<bool> MarkAsUsedAsync(string token);
    }
}
