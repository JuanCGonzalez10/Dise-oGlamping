using OmniPyme.Web.Core;
using OmniPyme.Web.DTOs;

namespace OmniPyme.Web.Services
{
    public interface IEmailService
    {
        public Task<Response<object>> SendAsync(SendEmailDTO dto);
        public Task<Response<object>> SendResetPasswordEmailAsync(string email, string message, string resetTokenLink);
    }
}
