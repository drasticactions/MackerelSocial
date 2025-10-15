// <copyright file="ThreadViewPostViewModelTests.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using FishyFlip;
using FishyFlip.Models;
using MackerelSocial.Core.Services;
using MackerelSocial.Core.ViewModels;

namespace MackerelSocial.Core.Tests;

/// <summary>
/// Tests for ThreadViewPostViewModel.
/// </summary>
public class ThreadViewPostViewModelTests : IDisposable
{
    private readonly ATProtocol protocol;
    private readonly DatabaseService database;

    public ThreadViewPostViewModelTests()
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
    public void Constructor_WithUri_InitializesCorrectly()
    {
        // Arrange
        var uri = new ATUri(TestConstants.TestPostUri);

        // Act
        var viewModel = new ThreadViewPostViewModel(uri, this.protocol, this.database);

        // Assert
        Assert.NotNull(viewModel);
        Assert.NotNull(viewModel.Protocol);
        Assert.NotNull(viewModel.Database);
        Assert.Null(viewModel.Post);
    }

    [Fact]
    public async Task RefreshAsync_WithValidUri_LoadsPost()
    {
        // Arrange
        var uri = new ATUri(TestConstants.TestPostUri);
        var viewModel = new ThreadViewPostViewModel(uri, this.protocol, this.database);
        using var cts = new CancellationTokenSource(TestConstants.DefaultTimeoutMs);

        // Act
        await viewModel.RefreshAsync(cts.Token);

        // Assert
        Assert.NotNull(viewModel.Post);
        Assert.NotNull(viewModel.Post.Post);
        // Note: URI may be normalized from handle to DID format
        Assert.Contains("3m367otk7mc2e", viewModel.Post.Post.Uri.ToString());
    }

    [Fact]
    public async Task RefreshAsync_WithInvalidUri_HandlesGracefully()
    {
        // Arrange
        var invalidUri = new ATUri("at://did:plc:invalid/app.bsky.feed.post/invalid");
        var viewModel = new ThreadViewPostViewModel(invalidUri, this.protocol, this.database);
        using var cts = new CancellationTokenSource(TestConstants.DefaultTimeoutMs);

        // Act & Assert - Should not throw, but Post may remain null
        await viewModel.RefreshAsync(cts.Token);

        // The Post may be null or the operation may have been handled by error messaging
        // We're just verifying it doesn't throw an unhandled exception
    }

    [Fact]
    public void Dispose_DisposesViewModel()
    {
        // Arrange
        var uri = new ATUri(TestConstants.TestPostUri);
        var viewModel = new ThreadViewPostViewModel(uri, this.protocol, this.database);

        // Act & Assert - Should not throw
        viewModel.Dispose();
    }
}
