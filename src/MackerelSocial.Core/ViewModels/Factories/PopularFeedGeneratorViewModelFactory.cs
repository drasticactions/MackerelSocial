using FishyFlip;
using MackerelSocial.Services;

namespace MackerelSocial.ViewModels.Factories;

public class PopularFeedGeneratorViewModelFactory : IPopularFeedGeneratorViewModelFactory
{
    private readonly ATProtocol protocol;
    private readonly DatabaseService database;

    public PopularFeedGeneratorViewModelFactory(ATProtocol protocol, DatabaseService database)
    {
        this.protocol = protocol ?? throw new ArgumentNullException(nameof(protocol));
        this.database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public PopularFeedGeneratorViewModel Create()
    {
        return new PopularFeedGeneratorViewModel(protocol, database);
    }

    public PopularFeedGeneratorViewModel Create(string query)
    {
        return new PopularFeedGeneratorViewModel(query, protocol, database);
    }
}