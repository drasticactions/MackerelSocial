// <copyright file="PopularFeedGeneratorCollectionTests.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using FishyFlip;
using MackerelSocial.Core.Collections;

namespace MackerelSocial.Core.Tests;

/// <summary>
/// Tests for PopularFeedGeneratorCollection.
/// </summary>
public class PopularFeedGeneratorCollectionTests
{
    private readonly ATProtocol protocol;

    public PopularFeedGeneratorCollectionTests()
    {
        var builder = new ATProtocolBuilder();
        this.protocol = builder.Build();
    }

    [Fact]
    public async Task GetMoreItemsAsync_LoadsPopularGenerators()
    {
        // Arrange
        var collection = new PopularFeedGeneratorCollection(this.protocol);
        using var cts = new CancellationTokenSource(TestConstants.DefaultTimeoutMs);

        // Act
        await collection.GetMoreItemsAsync(10, cts.Token);

        // Assert
        Assert.True(collection.Count > 0, "Should have loaded popular generators");
    }

    [Fact]
    public async Task GetMoreItemsAsync_WithQuery_LoadsFilteredGenerators()
    {
        // Arrange
        var collection = new PopularFeedGeneratorCollection(TestConstants.TestSearchQuery, this.protocol);
        using var cts = new CancellationTokenSource(TestConstants.DefaultTimeoutMs);

        // Act
        await collection.GetMoreItemsAsync(10, cts.Token);

        // Assert
        Assert.True(collection.Count > 0, $"Should have loaded generators matching '{TestConstants.TestSearchQuery}'");
        Assert.Equal(TestConstants.TestSearchQuery, collection.Query);
    }

    [Fact]
    public void Query_WhenSet_ClearsCollectionAndUpdatesQuery()
    {
        // Arrange
        var collection = new PopularFeedGeneratorCollection(this.protocol);

        // Act
        collection.Query = "newquery";

        // Assert
        Assert.Equal("newquery", collection.Query);
        Assert.Equal(0, collection.Count);
    }

    [Fact]
    public async Task RefreshAsync_ClearsPreviousDataAndReloads()
    {
        // Arrange
        var collection = new PopularFeedGeneratorCollection(this.protocol);
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
    public async Task GetRecordsAsync_ReturnsGeneratorsAndCursor()
    {
        // Arrange
        var collection = new PopularFeedGeneratorCollection(this.protocol);
        using var cts = new CancellationTokenSource(TestConstants.DefaultTimeoutMs);

        // Act
        var (generators, cursor) = await collection.GetRecordsAsync(10, cts.Token);

        // Assert
        Assert.NotNull(generators);
        Assert.True(generators.Count > 0, "Should return generators");
        Assert.NotNull(cursor);
    }

    [Fact]
    public async Task GetMoreItemsAsync_WithPagination_LoadsMultiplePages()
    {
        // Arrange
        var collection = new PopularFeedGeneratorCollection(this.protocol);
        using var cts = new CancellationTokenSource(TestConstants.DefaultTimeoutMs);

        // Act - Load first page
        await collection.GetMoreItemsAsync(5, cts.Token);
        var firstPageCount = collection.Count;
        var firstCursor = collection.Cursor;

        // Load second page if available
        if (collection.HasMoreItems)
        {
            await collection.GetMoreItemsAsync(5, cts.Token);
            var secondPageCount = collection.Count;

            // Assert
            Assert.True(secondPageCount > firstPageCount, "Should have more items after second page");
            Assert.NotNull(firstCursor);
            Assert.NotEmpty(firstCursor);
        }
    }
}
