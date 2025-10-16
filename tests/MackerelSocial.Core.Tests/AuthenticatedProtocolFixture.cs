using FishyFlip;
using FishyFlip.Models;
using MackerelSocial.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MackerelSocial.Core.Tests;

public class AuthenticatedProtocolFixture : IDisposable
{
    public ATProtocol Protocol { get; private set; }

    public LoginUser CurrentUser { get; private set; }
    
    public AuthenticatedProtocolFixture()
    {
        var builder = new ATProtocolBuilder();
        string handle = Environment.GetEnvironmentVariable("BLUESKY_TEST_HANDLE") ?? throw new ArgumentNullException();
        string password = Environment.GetEnvironmentVariable("BLUESKY_TEST_PASSWORD") ?? throw new ArgumentNullException();
        this.Protocol = builder.Build();
        var task = this.Protocol.AuthenticateWithPasswordResultAsync(handle, password);
        var (result, _) = task.GetAwaiter().GetResult();
        this.CurrentUser = new Models.LoginUser
        {
            Handle = result!.Handle.Handle,
            Email = result.Email ?? string.Empty,
            Did = result.Did.ToString(),
            SessionData = JsonSerializer.Serialize<Session>(result, this.Protocol.Options.JsonSerializerOptions),
            LoginType = Models.LoginType.Password
        };
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
