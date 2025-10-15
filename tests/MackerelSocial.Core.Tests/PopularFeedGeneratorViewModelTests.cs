// <copyright file="PopularFeedGeneratorViewModelTests.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using FishyFlip;
using MackerelSocial.Core.Services;
using MackerelSocial.Core.ViewModels;

namespace MackerelSocial.Core.Tests;

/// <summary>
/// Tests for PopularFeedGeneratorViewModel.
/// </summary>
public class PopularFeedGeneratorViewModelTests : IDisposable
{
    private readonly ATProtocol protocol;
    private readonly DatabaseService database;

    public PopularFeedGeneratorViewModelTests()
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
    public void Constructor_WithoutQuery_InitializesGeneratorsCollection()
    {
        // Act
        var viewModel = new PopularFeedGeneratorViewModel(this.protocol, this.database);

        // Assert
        Assert.NotNull(viewModel);
        Assert.NotNull(viewModel.Generators);
        Assert.NotNull(viewModel.Protocol);
        Assert.NotNull(viewModel.Database);
        Assert.Equal(string.Empty, viewModel.Query);
    }

    [Fact]
    public void Constructor_WithQuery_InitializesWithQuery()
    {
        // Act
        var viewModel = new PopularFeedGeneratorViewModel(TestConstants.TestSearchQuery, this.protocol, this.database);

        // Assert
        Assert.NotNull(viewModel);
        Assert.NotNull(viewModel.Generators);
        Assert.Equal(TestConstants.TestSearchQuery, viewModel.Generators.Query);
    }

    [Fact]
    public async Task RefreshGeneratorsAsync_LoadsGenerators()
    {
        // Arrange
        var viewModel = new PopularFeedGeneratorViewModel(this.protocol, this.database);

        // Act
        await viewModel.RefreshGeneratorsAsync();

        // Assert
        Assert.NotNull(viewModel.Generators);
        Assert.True(viewModel.Generators.Count > 0, "Should have loaded some popular generators");
        Assert.False(viewModel.IsRefreshing);
    }

    [Fact]
    public async Task RefreshGeneratorsAsync_WithQuery_LoadsFilteredGenerators()
    {
        // Arrange
        var viewModel = new PopularFeedGeneratorViewModel(TestConstants.TestSearchQuery, this.protocol, this.database);

        // Act
        await viewModel.RefreshGeneratorsAsync();

        // Assert
        Assert.NotNull(viewModel.Generators);
        Assert.True(viewModel.Generators.Count > 0, $"Should have loaded generators matching '{TestConstants.TestSearchQuery}'");
        Assert.False(viewModel.IsRefreshing);
    }

    [Fact]
    public async Task RefreshGeneratorsAsync_WhenAlreadyRefreshing_DoesNotStartNewRefresh()
    {
        // Arrange
        var viewModel = new PopularFeedGeneratorViewModel(this.protocol, this.database);

        // Act - Start first refresh
        var firstRefresh = viewModel.RefreshGeneratorsAsync();

        // Immediately try to start second refresh while first is running
        var secondRefresh = viewModel.RefreshGeneratorsAsync();

        await Task.WhenAll(firstRefresh, secondRefresh);

        // Assert - Both should complete without errors
        Assert.False(viewModel.IsRefreshing);
    }

    [Fact]
    public void Dispose_DisposesViewModel()
    {
        // Arrange
        var viewModel = new PopularFeedGeneratorViewModel(this.protocol, this.database);

        // Act & Assert - Should not throw
        viewModel.Dispose();
    }
}
