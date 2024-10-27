using System;

namespace EcbGateway;

public class EcbGatewayException : Exception
{
    public EcbGatewayException() : base()
    {
    }

    public EcbGatewayException(string? message) : base(message)
    {
    }

    public EcbGatewayException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
