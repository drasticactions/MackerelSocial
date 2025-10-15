// <copyright file="RepoViewModelTests.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using FishyFlip;
using FishyFlip.Models;
using MackerelSocial.Core.Services;
using MackerelSocial.Core.ViewModels;

namespace MackerelSocial.Core.Tests;

/// <summary>
/// Tests for RepoViewModel.
/// </summary>
public class RepoViewModelTests : IDisposable
{
    private readonly ATProtocol protocol;
    private readonly DatabaseService database;

    public RepoViewModelTests()
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
    public async Task OpenRepoFromIdentifierAsync_WithValidIdentifier_LoadsATObjects()
    {
        // Arrange
        var identifier = ATIdentifier.Create(TestConstants.TestHandle)!;
        var viewModel = new RepoViewModel(this.protocol, this.database);
        using var cts = new CancellationTokenSource(TestConstants.DefaultTimeoutMs);

        // Act
        await viewModel.OpenRepoFromIdentifierAsync(identifier, cts.Token);

        // Assert
        Assert.True(viewModel.ATObjects.Count > 0, "Should have loaded AT objects from repository");
        Assert.False(viewModel.IsBusy);
    }

    [Fact]
    public async Task OpenRepoFromIdentifierAsync_WithInvalidIdentifier_HandlesGracefully()
    {
        // Arrange
        var invalidIdentifier = ATIdentifier.Create("invalid.handle.that.does.not.exist")!;
        var viewModel = new RepoViewModel(this.protocol, this.database);
        using var cts = new CancellationTokenSource(TestConstants.DefaultTimeoutMs);

        // Act & Assert - Should not throw, but may handle error internally
        await viewModel.OpenRepoFromIdentifierAsync(invalidIdentifier, cts.Token);

        // The operation should complete without throwing
        Assert.False(viewModel.IsBusy);
    }

    [Fact]
    public async Task OpenRepoStreamAsync_WithNullStream_ThrowsArgumentNullException()
    {
        // Arrange
        var viewModel = new RepoViewModel(this.protocol, this.database);
        using var cts = new CancellationTokenSource(TestConstants.DefaultTimeoutMs);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await viewModel.OpenRepoStreamAsync(null!, cts.Token));
    }

    [Fact]
    public void SelectedATObject_CanBeSetAndRetrieved()
    {
        // Arrange
        var viewModel = new RepoViewModel(this.protocol, this.database);

        // Act
        viewModel.SelectedATObject = null;

        // Assert
        Assert.Null(viewModel.SelectedATObject);
    }

    [Fact]
    public void Dispose_DisposesViewModel()
    {
        // Arrange
        var viewModel = new RepoViewModel(this.protocol, this.database);

        // Act & Assert - Should not throw
        viewModel.Dispose();
    }
}
