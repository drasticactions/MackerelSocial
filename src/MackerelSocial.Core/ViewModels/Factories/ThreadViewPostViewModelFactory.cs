using FishyFlip;
using FishyFlip.Lexicon.App.Bsky.Feed;
using FishyFlip.Models;
using MackerelSocial.Core.Services;
using Microsoft.Extensions.Logging;

namespace MackerelSocial.Core.ViewModels.Factories;

public class ThreadViewPostViewModelFactory : IThreadViewPostViewModelFactory
{
    private readonly ATProtocol protocol;
    private readonly DatabaseService database;
    private readonly ILogger<ThreadViewPostViewModelFactory>? logger;

    public ThreadViewPostViewModelFactory(ATProtocol protocol, DatabaseService database, ILogger<ThreadViewPostViewModelFactory>? logger = null)
    {
        this.protocol = protocol ?? throw new ArgumentNullException(nameof(protocol));
        this.database = database ?? throw new ArgumentNullException(nameof(database));
        this.logger = logger;
    }

    public ThreadViewPostViewModel Create(ThreadViewPost post, Models.LoginUser? currentUser = default)
    {
        return new ThreadViewPostViewModel(post, protocol, database, currentUser, logger);
    }

    public ThreadViewPostViewModel Create(ATUri uri, Models.LoginUser? currentUser = default)
    {
        return new ThreadViewPostViewModel(uri, protocol, database, currentUser, logger);
    }
}