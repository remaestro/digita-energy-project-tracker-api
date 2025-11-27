using System.Threading.Tasks;

namespace DigitaEnergy.ProjectTracker.Application.Interfaces;

public interface IEmailService
{
    Task SendInvitationEmailAsync(string toEmail, string firstName, string lastName, string invitationToken, string invitedByName);
    Task SendPasswordResetEmailAsync(string toEmail, string resetToken);
    Task SendWelcomeEmailAsync(string toEmail, string firstName);
}
