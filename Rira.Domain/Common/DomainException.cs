namespace Rira.Domain.Common;

public sealed class DomainException : Exception
{
    public DomainException(DomainError error) : this([error])
    {
    }

    public DomainException(IReadOnlyList<DomainError> errors) : base(string.Join("; ", errors.Select(error => error.Message)))
    {
        Errors = errors;
    }

    public IReadOnlyList<DomainError> Errors { get; }
}
