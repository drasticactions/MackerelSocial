using FishyFlip;
using FishyFlip.Models;

namespace MackerelSocial.Core.ViewModels.Factories;

public class AuthorViewModelFactory : IAuthorViewModelFactory
{
    private readonly ATProtocol protocol;
    private readonly Services.DatabaseService database;

    public AuthorViewModelFactory(ATProtocol protocol, Services.DatabaseService database)
    {
        this.protocol = protocol ?? throw new ArgumentNullException(nameof(protocol));
        this.database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public AuthorViewModel Create(ATIdentifier identifier, string filter = "")
    {
        return new AuthorViewModel(identifier, protocol, database, filter);
    }
}