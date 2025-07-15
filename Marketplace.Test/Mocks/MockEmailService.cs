using Marketplace.Core.Services;
using Moq;

namespace Marketplace.Test.Mocks;

public class MockEmailService : Mock<IEmailService>
{
    public List<EmailSentRecord> EmailsSent { get; } = new();
    
    public MockEmailService()
    {
        Setup(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask)
            .Callback<string, string>((email, link) => 
                EmailsSent.Add(new EmailSentRecord(email, link)));
    }
    
    public void SetupSendEmailFailure()
    {
        Setup(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new InvalidOperationException("Email sending failed"));
    }
    
    public void VerifyEmailSent(string email, string confirmationLink)
    {
        var emailRecord = EmailsSent.FirstOrDefault(e => e.Email == email && e.ConfirmationLink == confirmationLink);
        if (emailRecord == null)
        {
            throw new InvalidOperationException($"Expected email to {email} with confirmation link {confirmationLink} was not sent");
        }
    }
    
    public void VerifyNoEmailSent()
    {
        if (EmailsSent.Any())
        {
            throw new InvalidOperationException($"Expected no emails to be sent, but {EmailsSent.Count} were sent");
        }
    }
}

public record EmailSentRecord(string Email, string ConfirmationLink);
