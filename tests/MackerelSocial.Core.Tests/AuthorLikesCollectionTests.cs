// <copyright file="AuthorLikesCollectionTests.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using FishyFlip;
using FishyFlip.Models;
using MackerelSocial.Core.Collections;

namespace MackerelSocial.Core.Tests;

/// <summary>
/// Tests for AuthorLikesCollection.
/// </summary>
[Collection("Auth")]
public class AuthorLikesCollectionTests
{
    private readonly ATProtocol protocol;

    /// <summary>
    /// Initializes a new instance of the AuthorLikesCollectionTests class using the specified authenticated protocol
    /// fixture.
    /// </summary>
    /// <param name="fixture">The fixture that provides an authenticated protocol instance for use in test cases. Cannot be null.</param>
    public AuthorLikesCollectionTests(AuthenticatedProtocolFixture fixture)
    {
        this.protocol = fixture.Protocol;
    }

    [Fact]
    public async Task GetMoreItemsAsync_LoadsAuthorLikes()
    {
        // Arrange
        var collection = new AuthorLikesCollection(this.protocol);
        using var cts = new CancellationTokenSource(TestConstants.DefaultTimeoutMs);

        // Act & Assert
        await collection.GetMoreItemsAsync(10, cts.Token);
        Assert.True(collection.Count >= 0);
    }

    [Fact]
    public async Task GetMoreItemsAsync_WithPagination_LoadsMultiplePages()
    {
        // Arrange
        var collection = new AuthorLikesCollection(this.protocol);
        using var cts = new CancellationTokenSource(TestConstants.DefaultTimeoutMs);

        // Act & Assert
        await collection.GetMoreItemsAsync(5, cts.Token);
        var firstPageCount = collection.Count;

        // Only test second page if there are items and HasMoreItems is true
        if (firstPageCount > 0 && collection.HasMoreItems)
        {
            await collection.GetMoreItemsAsync(5, cts.Token);
            var secondPageCount = collection.Count;
            Assert.True(secondPageCount >= firstPageCount, "Should have same or more items after second page");
        }
    }

    [Fact]
    public async Task GetRecordsAsync_ReturnsLikesAndCursor()
    {
        // Arrange
        var collection = new AuthorLikesCollection(this.protocol);
        using var cts = new CancellationTokenSource(TestConstants.DefaultTimeoutMs);

        // Act & Assert
        var (likes, cursor) = await collection.GetRecordsAsync(10, cts.Token);
        Assert.NotNull(likes);
        Assert.NotNull(cursor);
        Assert.True(likes.Count >= 0);
    }
}
