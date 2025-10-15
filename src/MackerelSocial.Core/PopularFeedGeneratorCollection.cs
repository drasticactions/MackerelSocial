// <copyright file="PopularFeedGeneratorCollection.cs" company="Drastic Actions">
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

public class PopularFeedGeneratorCollection : ATObjectCollectionBase<GeneratorView>, IAsyncEnumerable<GeneratorView>
{
    private ATProtocol atp;
    private string query = string.Empty;

    public PopularFeedGeneratorCollection(string query, ATProtocol atp)
        : base(atp)
    {
        this.atp = atp;
        this.query = query;
    }

    public PopularFeedGeneratorCollection(ATProtocol atp)
        : base(atp)
    {
        this.atp = atp;
        this.query = string.Empty;
    }

    public string Query
    {
        get => this.query;
        set
        {
            this.Clear();
            if (this.query != value)
            {
                this.query = value;
            }
        }
    }

    public ATProtocol ATProtocol => this.atp;

    public override async Task<(IList<GeneratorView> Posts, string Cursor)> GetRecordsAsync(int? limit = null, CancellationToken? cancellationToken = null)
    {
        cancellationToken?.ThrowIfCancellationRequested();
        var (result, error) = await this.ATProtocol.Unspecced.GetPopularFeedGeneratorsAsync(limit, this.Cursor, this.Query, cancellationToken ?? System.Threading.CancellationToken.None);
        this.HandleATError(error);
        if (result == null || result.Feeds == null)
        {
            throw new InvalidOperationException("The result or its properties cannot be null.");
        }

        return (result.Feeds, result.Cursor ?? string.Empty);
    }
    
    /// <inheritdoc/>
    public override Task RefreshAsync(int? limit = null, CancellationToken? cancellationToken = null)
    {
        cancellationToken?.ThrowIfCancellationRequested();
        this.Clear();
        return this.GetMoreItemsAsync(limit, cancellationToken);
    }
}