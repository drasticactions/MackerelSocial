using FishyFlip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MackerelSocial.Core.Tests;

public class AuthenticatedProtocolFixture : IDisposable
{
    public ATProtocol Protocol { get; private set; }
    public AuthenticatedProtocolFixture()
    {
        var builder = new ATProtocolBuilder();
        string handle = Environment.GetEnvironmentVariable("BLUESKY_TEST_HANDLE") ?? throw new ArgumentNullException();
        string password = Environment.GetEnvironmentVariable("BLUESKY_TEST_PASSWORD") ?? throw new ArgumentNullException();
        this.Protocol = builder.Build();
        this.Protocol.AuthenticateWithPasswordResultAsync(handle, password).Wait();
    }
    public void Dispose()
    {
        this.Protocol.Dispose();
    }
}

[CollectionDefinition("Auth")]
public class AuthenticatedProtocolCollection : ICollectionFixture<AuthenticatedProtocolFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
