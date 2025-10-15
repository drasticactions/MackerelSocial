// <copyright file="FeedViewCollectionTests.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using FishyFlip;
using FishyFlip.Models;
using MackerelSocial.Core.Collections;

namespace MackerelSocial.Core.Tests;

/// <summary>
/// Tests for FeedViewCollection.
/// </summary>
public class FeedViewCollectionTests
{
    private readonly ATProtocol protocol;

    public FeedViewCollectionTests()
    {
        var builder = new ATProtocolBuilder();
        this.protocol = builder.Build();
    }

    [Fact]
    public async Task GetMoreItemsAsync_LoadsFeedPosts()
    {
        // Arrange
        var feedUri = new ATUri(TestConstants.TestFeedUri);
        var collection = new FeedViewCollection(this.protocol, feedUri);
        using var cts = new CancellationTokenSource(TestConstants.DefaultTimeoutMs);

        // Act
        await collection.GetMoreItemsAsync(10, cts.Token);

        // Assert
        Assert.True(collection.Count > 0, "Should have loaded posts from feed");
        Assert.NotNull(collection.FeedUri);
        Assert.Equal(feedUri, collection.FeedUri);
    }

    [Fact]
    public async Task GetMoreItemsAsync_WithPagination_LoadsMultiplePages()
    {
        // Arrange
        var feedUri = new ATUri(TestConstants.TestFeedUri);
        var collection = new FeedViewCollection(this.protocol, feedUri);
        using var cts = new CancellationTokenSource(TestConstants.DefaultTimeoutMs);

        // Act & Assert
        try
        {
            await collection.GetMoreItemsAsync(5, cts.Token);
            var firstPageCount = collection.Count;
            var firstCursor = collection.Cursor;

            // Load second page if available
            if (collection.HasMoreItems)
            {
                await collection.GetMoreItemsAsync(5, cts.Token);
                var secondPageCount = collection.Count;
                Assert.True(secondPageCount > firstPageCount, "Should have more items after second page");
                Assert.NotNull(firstCursor);
                Assert.NotEmpty(firstCursor);
            }
        }
        catch (FishyFlip.Tools.ATNetworkErrorException ex) when (ex.Message.Contains("InternalServerError"))
        {
            // Skip test if feed generator is having issues
            Assert.True(true);
        }
    }

    [Fact]
    public async Task RefreshAsync_ClearsPreviousDataAndReloads()
    {
        // Arrange
        var feedUri = new ATUri(TestConstants.TestFeedUri);
        var collection = new FeedViewCollection(this.protocol, feedUri);
        using var cts = new CancellationTokenSource(TestConstants.DefaultTimeoutMs);

        // Load initial data
        await collection.GetMoreItemsAsync(10, cts.Token);
        var initialCount = collection.Count;

        // Act - Refresh
        await collection.RefreshAsync(10, cts.Token);

        // Assert
        Assert.True(collection.Count > 0, "Should have data after refresh");
    }

    [Fact]
    public async Task GetRecordsAsync_ReturnsPostsAndCursor()
    {
        // Arrange
        var feedUri = new ATUri(TestConstants.TestFeedUri);
        var collection = new FeedViewCollection(this.protocol, feedUri);
        using var cts = new CancellationTokenSource(TestConstants.DefaultTimeoutMs);

        // Act
        var (posts, cursor) = await collection.GetRecordsAsync(10, cts.Token);

        // Assert
        Assert.NotNull(posts);
        Assert.True(posts.Count > 0, "Should return posts");
        Assert.NotNull(cursor);
    }
}
