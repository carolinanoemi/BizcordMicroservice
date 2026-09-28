namespace MessagingService.Domain.Exceptions;

/// <summary>
/// A custom exception for when something breaks a business rule.
/// For example: trying to create a message with empty content.
/// We use this instead of the generic "Exception" class so we can
/// tell the difference between "our validation caught a problem"
/// and "something unexpected crashed".
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}