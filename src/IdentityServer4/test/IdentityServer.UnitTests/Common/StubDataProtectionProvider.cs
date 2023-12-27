using Microsoft.AspNetCore.DataProtection;

namespace IdentityServer.UnitTests.Common;

internal class StubDataProtectionProvider : IDataProtectionProvider, IDataProtector
{
    public IDataProtector CreateProtector(string purpose)
    {
        return this;
    }

    public byte[] Protect(byte[] plaintext)
    {
        return plaintext;
    }

    public byte[] Unprotect(byte[] protectedData)
    {
        return protectedData;
    }
}