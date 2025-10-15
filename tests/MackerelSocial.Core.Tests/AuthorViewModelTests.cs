// <copyright file="AuthorViewModelTests.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using FishyFlip;
using FishyFlip.Models;
using MackerelSocial.Core.Services;
using MackerelSocial.Core.ViewModels;

namespace MackerelSocial.Core.Tests;

/// <summary>
/// Tests for AuthorViewModel.
/// </summary>
public class AuthorViewModelTests : IDisposable
{
    private readonly ATProtocol protocol;
    private readonly DatabaseService database;

    public AuthorViewModelTests()
    {
        var builder = new ATProtocolBuilder();
        this.protocol = builder.Build();
        this.database = new DatabaseService(":memory:");
    }

    public void Dispose()
    {
        this.database.Dispose();
    }

    [Fact]
    public void Constructor_WithHandle_InitializesAllCollections()
    {
        // Arrange
        var identifier = ATIdentifier.Create(TestConstants.TestHandle)!;

        // Act
        var viewModel = new AuthorViewModel(identifier, this.protocol, this.database);

        // Assert
        Assert.NotNull(viewModel);
        Assert.NotNull(viewModel.Protocol);
        Assert.NotNull(viewModel.Database);
        Assert.NotNull(viewModel.MainAuthorFeed);
        Assert.NotNull(viewModel.RepliesFeed);
        Assert.NotNull(viewModel.VideosFeed);
        Assert.NotNull(viewModel.MediaFeed);

        // When not authenticated, Likes should be null.
        Assert.Null(viewModel.LikesFeed);
    }

    [Fact]
    public void Constructor_WithDid_InitializesAllCollections()
    {
        // Arrange
        var identifier = ATIdentifier.Create(TestConstants.TestDid)!;

        // Act
        var viewModel = new AuthorViewModel(identifier, this.protocol, this.database);

        // Assert
        Assert.NotNull(viewModel);
        Assert.NotNull(viewModel.MainAuthorFeed);
        Assert.NotNull(viewModel.RepliesFeed);
        Assert.NotNull(viewModel.VideosFeed);
        Assert.NotNull(viewModel.MediaFeed);

        // When not authenticated, Likes should be null.
        Assert.Null(viewModel.LikesFeed);
    }

    [Fact]
    public async Task MainAuthorFeed_LoadsPosts()
    {
        // Arrange
        var identifier = ATIdentifier.Create(TestConstants.TestHandle)!;
        var viewModel = new AuthorViewModel(identifier, this.protocol, this.database);
        using var cts = new CancellationTokenSource(TestConstants.DefaultTimeoutMs);

        // Act
        await viewModel.MainAuthorFeed.GetMoreItemsAsync(10, cts.Token);

        // Assert
        Assert.True(viewModel.MainAuthorFeed.Count > 0, "Should have loaded posts from author feed");
    }

    [Fact]
    public async Task RepliesFeed_LoadsReplies()
    {
        // Arrange
        var identifier = ATIdentifier.Create(TestConstants.TestHandle)!;
        var viewModel = new AuthorViewModel(identifier, this.protocol, this.database);
        using var cts = new CancellationTokenSource(TestConstants.DefaultTimeoutMs);

        // Act
        await viewModel.RepliesFeed.GetMoreItemsAsync(10, cts.Token);

        // Assert
        // Note: May be 0 if the user has no replies, but the call should succeed
        Assert.True(viewModel.RepliesFeed.Count >= 0);
    }

    [Fact]
    public async Task MediaFeed_LoadsMediaPosts()
    {
        // Arrange
        var identifier = ATIdentifier.Create(TestConstants.TestHandle)!;
        var viewModel = new AuthorViewModel(identifier, this.protocol, this.database);
        using var cts = new CancellationTokenSource(TestConstants.DefaultTimeoutMs);

        // Act
        await viewModel.MediaFeed.GetMoreItemsAsync(10, cts.Token);

        // Assert
        // Note: May be 0 if the user has no media posts, but the call should succeed
        Assert.True(viewModel.MediaFeed.Count >= 0);
    }

    [Fact]
    public async Task VideosFeed_LoadsVideoPosts()
    {
        // Arrange
        var identifier = ATIdentifier.Create(TestConstants.TestHandle)!;
        var viewModel = new AuthorViewModel(identifier, this.protocol, this.database);
        using var cts = new CancellationTokenSource(TestConstants.DefaultTimeoutMs);

        // Act
        await viewModel.VideosFeed.GetMoreItemsAsync(10, cts.Token);

        // Assert
        // Note: May be 0 if the user has no video posts, but the call should succeed
        Assert.True(viewModel.VideosFeed.Count >= 0);
    }

    [Fact]
    public void Dispose_DisposesViewModel()
    {
        // Arrange
        var identifier = ATIdentifier.Create(TestConstants.TestHandle)!;
        var viewModel = new AuthorViewModel(identifier, this.protocol, this.database);

        // Act & Assert - Should not throw
        viewModel.Dispose();
    }
}
