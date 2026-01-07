using System;

namespace Breizh360.Domaine.Common;

public sealed class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}
