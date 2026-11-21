namespace Rira.Domain.Common;

public sealed record DomainError(string Field, string Code, string Message);
