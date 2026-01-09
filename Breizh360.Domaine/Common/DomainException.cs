namespace Breizh360.Domaine.Common;

/// <summary>
/// Exception métier du domaine (invariants, règles).
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
    public DomainException(string message, Exception inner) : base(message, inner) { }
}
