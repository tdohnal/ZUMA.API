using ZUMA.BussinessLogic.Entities;

namespace ZUMA.BussinessLogic.Models;

/// <summary>
/// Represents the result of a validation operation for an auditable entity, indicating whether the validation was
/// successful and providing an associated error message if it was not.
/// </summary>
/// <remarks>Use this class to encapsulate the outcome of validating an entity, including the validity status, any
/// error message, and the entity itself. The IsValid property indicates whether the validation succeeded. If validation
/// fails, ErrorMessage contains details about the failure.</remarks>
/// <typeparam name="T">The type of the entity being validated. Must implement the IAuditableEntities interface.</typeparam>
public class ValidationResult<T> where T : IAuditableEntities
{
    public ValidationResult(string message = "")
    {
        ErrorMessage = message;
    }

    public bool IsValid => ErrorMessage?.Length == 0;

    public string? ErrorMessage { get; private set; }

    public T? Entity { get; set; }

    public string Token { get; set; }
}
