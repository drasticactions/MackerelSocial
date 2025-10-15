using FishyFlip;
using FishyFlip.Models;
using MackerelSocial.Core.Collections;
using MackerelSocial.Core.Events;
using MackerelSocial.Core.Services;

namespace MackerelSocial.Core.ViewModels;

public partial class AuthorViewModel : BaseViewModel
{
    private ATIdentifier atIdentifier;

    public AuthorViewModel(ATIdentifier identifier, ATProtocol protocol, DatabaseService database)
        : base(protocol, database)
    {
        this.atIdentifier = identifier;
        this.MainAuthorFeed = new AuthorViewCollection(protocol, identifier, AuthorFilterConstants.PostsAndAuthorThreads, true);
        this.RepliesFeed = new AuthorViewCollection(protocol, identifier, AuthorFilterConstants.PostsWithReplies, false);
        this.VideosFeed = new AuthorViewCollection(protocol, identifier, AuthorFilterConstants.PostsWithVideo, false);
        this.MediaFeed = new AuthorViewCollection(protocol, identifier, AuthorFilterConstants.PostsWithMedia, false);
    }
    
    public AuthorViewCollection MainAuthorFeed { get; }

    public AuthorViewCollection RepliesFeed { get; }

    public AuthorViewCollection VideosFeed { get; }

    public AuthorViewCollection MediaFeed { get; }
}