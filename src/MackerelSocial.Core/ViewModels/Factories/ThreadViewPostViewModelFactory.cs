using FishyFlip;
using FishyFlip.Lexicon.App.Bsky.Feed;
using FishyFlip.Models;
using MackerelSocial.Services;

namespace MackerelSocial.ViewModels.Factories;

public class ThreadViewPostViewModelFactory : IThreadViewPostViewModelFactory
{
    private readonly ATProtocol protocol;
    private readonly DatabaseService database;

    public ThreadViewPostViewModelFactory(ATProtocol protocol, DatabaseService database)
    {
        this.protocol = protocol ?? throw new ArgumentNullException(nameof(protocol));
        this.database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public ThreadViewPostViewModel Create(ThreadViewPost post)
    {
        return new ThreadViewPostViewModel(post, protocol, database);
    }

    public ThreadViewPostViewModel Create(ATUri uri)
    {
        return new ThreadViewPostViewModel(uri, protocol, database);
    }
}