using FishyFlip;
using FishyFlip.Lexicon.App.Bsky.Feed;
using FishyFlip.Models;

namespace MackerelSocial.Core.Collections;

/// <summary>
/// Author Likes Collection.
/// </summary>
public class AuthorLikesCollection : FeedViewPostCollection
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorLikesCollection"/> class.
    /// </summary>
    /// <param name="atProtocol">The ATProtocol.</param>
    /// <param name="identifier">The ATIdentifier.</param>
    public AuthorLikesCollection(ATProtocol atp, ATIdentifier identifier)
        : base(atp)
    {
        this.ATIdentifier = identifier;
    }

    /// <summary>
    /// Gets the ATIdentifier.
    /// </summary>
    public ATIdentifier ATIdentifier { get; }

    /// <inheritdoc/>
    public override async Task<(IList<FeedViewPost> Posts, string Cursor)> GetRecordsAsync(int? limit = null, CancellationToken? cancellationToken = null)
    {
        await this.GetMoreItemsAsync(limit, cancellationToken ?? System.Threading.CancellationToken.None);
        return (this.ToList(), this.Cursor ?? string.Empty);
    }

    /// <inheritdoc/>
    internal override async Task<(IList<FeedViewPost> Posts, string Cursor)> GetPostViewItemsAsync(int limit = 50, CancellationToken? token = default)
    {
        var (result, error) = await this.ATProtocol.Feed.GetActorLikesAsync(this.ATIdentifier, limit, this.Cursor, token ?? System.Threading.CancellationToken.None);

        this.HandleATError(error);
        if (result == null || result.Feed == null)
        {
            throw new InvalidOperationException("The result or its properties cannot be null.");
        }

        return (result.Feed, result.Cursor ?? string.Empty);
    }
}
