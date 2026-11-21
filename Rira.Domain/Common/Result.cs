namespace Rira.Domain.Common;

public sealed class Result<T>
{
    private Result(T? value, IReadOnlyList<DomainError> errors)
    {
        Value = value;
        Errors = errors;
    }

    public T? Value { get; }
    public IReadOnlyList<DomainError> Errors { get; }

    public bool IsSuccess()
    {
        return Errors.Count == 0;
    }

    public static Result<T> Success(T value)
    {
        return new(value, []);
    }

    public static Result<T> Failure(IReadOnlyList<DomainError> errors)
    {
        return new(default, errors);
    }
}
