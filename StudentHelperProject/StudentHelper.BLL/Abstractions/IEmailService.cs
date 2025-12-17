using System.Threading.Tasks;

namespace StudentHelper.BLL.Abstractions
{
    public interface IEmailService
    {
        Task<bool> SendPasswordResetCodeAsync(string email, string code, string userName);
        Task<bool> SendEmailAsync(string to, string subject, string body);
    }
}
