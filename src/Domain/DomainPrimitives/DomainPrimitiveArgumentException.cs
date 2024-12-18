﻿namespace CotdQualifierRank.Domain.DomainPrimitives;

public class DomainPrimitiveArgumentException<T> : ArgumentException
{
    public T? Value { get; }
    
    public DomainPrimitiveArgumentException()
    {
    }

    public DomainPrimitiveArgumentException(T value) : base($"The value {value} is not valid.")
    {
    }

    public DomainPrimitiveArgumentException(string message, T value) : base(message)
    {
        Value = value;
    }
}