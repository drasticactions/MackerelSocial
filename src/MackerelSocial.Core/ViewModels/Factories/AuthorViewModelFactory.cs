using FishyFlip;
using FishyFlip.Models;
using Microsoft.Extensions.Logging;

namespace MackerelSocial.Core.ViewModels.Factories;

public class AuthorViewModelFactory : IAuthorViewModelFactory
{
    private readonly ATProtocol protocol;
    private readonly Services.DatabaseService database;
    private readonly ILogger<AuthorViewModelFactory>? logger;

    public AuthorViewModelFactory(ATProtocol protocol, Services.DatabaseService database, ILogger<AuthorViewModelFactory>? logger = null)
    {
        this.protocol = protocol ?? throw new ArgumentNullException(nameof(protocol));
        this.database = database ?? throw new ArgumentNullException(nameof(database));
        this.logger = logger;
    }

    public AuthorViewModel Create(ATIdentifier identifier)
    {
        return new AuthorViewModel(identifier, protocol, database, logger);
    }
}