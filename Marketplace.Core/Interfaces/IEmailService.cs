namespace Marketplace.Core.Interfaces;

public interface IEmailService
{
    Task SendConfirmationEmailAsync(string email, string confirmationLink);
}