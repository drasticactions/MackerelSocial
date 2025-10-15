// <copyright file="FeedViewPostCollection.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using CommunityToolkit.Mvvm.ComponentModel;
using FishyFlip;
using FishyFlip.Lexicon;
using FishyFlip.Lexicon.App.Bsky.Feed;
using FishyFlip.Models;
using FishyFlip.Tools;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace MackerelSocial.Core;

/// <summary>
/// Enumerable collection of PostView objects.
/// </summary>
public abstract class FeedViewPostCollection : ATObjectCollectionBase<FeedViewPost>, IAsyncEnumerable<FeedViewPost>
{
    private ATProtocol atp;

    protected FeedViewPostCollection(ATProtocol atp) : base(atp)
    {
        this.atp = atp;
    }

    public ATProtocol ATProtocol => this.atp;

    /// <inheritdoc/>
    public override async Task GetMoreItemsAsync(int? limit = null, CancellationToken? cancellationToken = null)
    {
        cancellationToken?.ThrowIfCancellationRequested();
        var (postViews, cursor) = await this.GetPostViewItemsAsync(limit ?? 50, cancellationToken);
        foreach (var postView in postViews)
        {
            this.AddItem(postView);
        }

        this.HasMoreItems = !string.IsNullOrEmpty(cursor);
        this.Cursor = cursor;
    }

    /// <inheritdoc/>
    public override Task RefreshAsync(int? limit = null, CancellationToken? cancellationToken = null)
    {
        cancellationToken?.ThrowIfCancellationRequested();
        this.Clear();
        return this.GetMoreItemsAsync(limit, cancellationToken);
    }

    /// <summary>
    /// Get Post View Items.
    /// </summary>
    /// <param name="limit">Limit of items to fetch.</param>
    /// <param name="token">Cancellation Token.</param>
    /// <returns>Task.</returns>
    internal abstract Task<(IList<FeedViewPost> Posts, string Cursor)> GetPostViewItemsAsync(int limit = 50, CancellationToken? token = null);
}