using System;

namespace WalletApp.Core;

public class WalletAppException : Exception
{
    public WalletAppException() : base()
    {
    }

    public WalletAppException(string? message) : base(message)
    {
    }

    public WalletAppException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
