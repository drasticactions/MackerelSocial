using FishyFlip;
using MackerelSocial.Core.Services;
using Microsoft.Extensions.Logging;

namespace MackerelSocial.Core.ViewModels.Factories;

public class PopularFeedGeneratorViewModelFactory : IPopularFeedGeneratorViewModelFactory
{
    private readonly ATProtocol protocol;
    private readonly DatabaseService database;

    private readonly ILogger<PopularFeedGeneratorViewModelFactory>? logger;

    public PopularFeedGeneratorViewModelFactory(ATProtocol protocol, DatabaseService database, ILogger<PopularFeedGeneratorViewModelFactory>? logger = null)
    {
        this.protocol = protocol ?? throw new ArgumentNullException(nameof(protocol));
        this.database = database ?? throw new ArgumentNullException(nameof(database));
        this.logger = logger;
    }

    public PopularFeedGeneratorViewModel Create(Models.LoginUser? currentUser = default)
    {
        return new PopularFeedGeneratorViewModel(protocol, database, currentUser, logger);
    }

    public PopularFeedGeneratorViewModel Create(string query, Models.LoginUser? currentUser = default)
    {
        return new PopularFeedGeneratorViewModel(query, protocol, database, currentUser, logger);
    }
}