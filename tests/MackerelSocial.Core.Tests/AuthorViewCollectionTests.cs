// <copyright file="AuthorViewCollectionTests.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using FishyFlip;
using FishyFlip.Models;
using MackerelSocial.Core.Collections;

namespace MackerelSocial.Core.Tests;

/// <summary>
/// Tests for AuthorViewCollection.
/// </summary>
public class AuthorViewCollectionTests
{
    private readonly ATProtocol protocol;

    public AuthorViewCollectionTests()
    {
        var builder = new ATProtocolBuilder();
        this.protocol = builder.Build();
    }

    [Fact]
    public async Task GetMoreItemsAsync_LoadsAuthorPosts()
    {
        // Arrange
        var identifier = ATIdentifier.Create(TestConstants.TestHandle)!;
        var collection = new AuthorViewCollection(this.protocol, identifier, Core.AuthorFilterConstants.PostsAndAuthorThreads, true);
        using var cts = new CancellationTokenSource(TestConstants.DefaultTimeoutMs);

        // Act
        await collection.GetMoreItemsAsync(10, cts.Token);

        // Assert
        Assert.True(collection.Count > 0, "Should have loaded posts from author");
        Assert.NotNull(collection.ATIdentifier);
        Assert.Equal(identifier, collection.ATIdentifier);
    }

    [Fact]
    public async Task GetMoreItemsAsync_WithPagination_LoadsMultiplePages()
    {
        // Arrange
        var identifier = ATIdentifier.Create(TestConstants.TestHandle)!;
        var collection = new AuthorViewCollection(this.protocol, identifier);
        using var cts = new CancellationTokenSource(TestConstants.DefaultTimeoutMs);

        // Act - Load first page
        await collection.GetMoreItemsAsync(5, cts.Token);
        var firstPageCount = collection.Count;
        var firstCursor = collection.Cursor;

        // Load second page
        await collection.GetMoreItemsAsync(5, cts.Token);
        var secondPageCount = collection.Count;

        // Assert
        Assert.True(firstPageCount > 0, "Should have items from first page");
        Assert.True(secondPageCount > firstPageCount, "Should have more items after second page");
        Assert.NotNull(firstCursor);
        Assert.NotEmpty(firstCursor);
    }

    [Fact]
    public async Task RefreshAsync_ClearsPreviousDataAndReloads()
    {
        // Arrange
        var identifier = ATIdentifier.Create(TestConstants.TestHandle)!;
        var collection = new AuthorViewCollection(this.protocol, identifier);
        using var cts = new CancellationTokenSource(TestConstants.DefaultTimeoutMs);

        // Load initial data
        await collection.GetMoreItemsAsync(10, cts.Token);
        var initialCount = collection.Count;

        // Act - Refresh
        await collection.RefreshAsync(10, cts.Token);

        // Assert
        Assert.True(collection.Count > 0, "Should have data after refresh");
        // Cursor should be reset after refresh
        Assert.NotNull(collection.Cursor);
    }

    [Fact]
    public async Task GetRecordsAsync_ReturnsPostsAndCursor()
    {
        // Arrange
        var identifier = ATIdentifier.Create(TestConstants.TestHandle)!;
        var collection = new AuthorViewCollection(this.protocol, identifier);
        using var cts = new CancellationTokenSource(TestConstants.DefaultTimeoutMs);

        // Act
        var (posts, cursor) = await collection.GetRecordsAsync(10, cts.Token);

        // Assert
        Assert.NotNull(posts);
        Assert.True(posts.Count > 0, "Should return posts");
        Assert.NotNull(cursor);
    }

    [Fact]
    public void Constructor_WithFilter_SetsFilterCorrectly()
    {
        // Arrange & Act
        var identifier = ATIdentifier.Create(TestConstants.TestHandle)!;
        var collection = new AuthorViewCollection(
            this.protocol,
            identifier,
            Core.AuthorFilterConstants.PostsWithMedia,
            false);

        // Assert
        Assert.Equal(Core.AuthorFilterConstants.PostsWithMedia, collection.Filter);
        Assert.False(collection.IncludePins);
    }
}
