using Zuma.Customer.Domain.Entities;

public class VerificationResult
{
    public VerificationResult(string errorMessage = "")
    {
        ErrorMessage = errorMessage;
    }

    public UserEntity User { get; set; }
    public string Token { get; set; }
    public string? ErrorMessage { get; set; }

    public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);
}
