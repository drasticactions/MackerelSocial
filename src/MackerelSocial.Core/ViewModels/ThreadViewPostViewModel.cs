using System.Text.Json;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FishyFlip;
using FishyFlip.Lexicon.App.Bsky.Feed;
using FishyFlip.Models;
using MackerelSocial.Core.Services;
using Microsoft.Extensions.Logging;

namespace MackerelSocial.Core.ViewModels;

public partial class ThreadViewPostViewModel : BaseViewModel
{
    private ATUri _uri;
    public ThreadViewPostViewModel(ThreadViewPost post, ATProtocol protocol, DatabaseService database, ILogger? logger = null)
        : base(protocol, database, logger)
    {
        this.Post = post;
        this._uri = post.Post.Uri;
    }

    public ThreadViewPostViewModel(ATUri uri, ATProtocol protocol, DatabaseService database, ILogger? logger = null)
        : base(protocol, database, logger)
    {
        this._uri = uri;
    }

    [ObservableProperty]
    private ThreadViewPost? _post;

    public async Task RefreshAsync(CancellationToken? token = default)
    {
        var (post, error) = await this.Protocol.Feed.GetPostThreadAsync(this._uri, cancellationToken: token ?? CancellationToken.None);
        if (post?.Thread is ThreadViewPost thread)
        {
            this.Post = thread;
        }
    }
}