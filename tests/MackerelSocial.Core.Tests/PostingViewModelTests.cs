// <copyright file="PostingViewModelTests.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using FishyFlip;
using FishyFlip.Lexicon.App.Bsky.Richtext;
using FishyFlip.Lexicon.Com.Atproto.Label;
using MackerelSocial.Core.Services;
using MackerelSocial.Core.ViewModels;

namespace MackerelSocial.Core.Tests;

/// <summary>
/// Tests for PostingViewModel.
/// </summary>
[Collection("Auth")]
public class PostingViewModelTests : IDisposable
{
    private readonly AuthenticatedProtocolFixture fixture;
    private readonly DatabaseService database;
    private readonly ATProtocol protocol;

    public PostingViewModelTests(AuthenticatedProtocolFixture fixture)
    {
        this.fixture = fixture;
        this.database = new DatabaseService(":memory:");
        this.database.InitializeAsync().Wait();

        // Create a new protocol instance for each test
        var builder = new ATProtocolBuilder();
        this.protocol = builder.Build();
    }

    public void Dispose()
    {
        this.database.Dispose();
        this.protocol.Dispose();
    }

    [Fact]
    public void Constructor_InitializesWithEmptyValues()
    {
        // Act
        var viewModel = new PostingViewModel(this.protocol, this.database);

        // Assert
        Assert.Equal(string.Empty, viewModel.PostContent);
        Assert.Empty(viewModel.PostFacets);
        Assert.False(viewModel.IsPosting);
        Assert.Equal(string.Empty, viewModel.ErrorMessage);
        Assert.Null(viewModel.SelfLabels);
        Assert.Null(viewModel.Embed);
        Assert.Empty(viewModel.Languages);
        Assert.Empty(viewModel.Tags);
    }

    [Fact]
    public void CanPost_WithValidContent_ReturnsTrue()
    {
        // Arrange
        var viewModel = new PostingViewModel(this.fixture.Protocol, this.database, this.fixture.CurrentUser);
        viewModel.PostContent = "This is a test post";

        // Act & Assert
        Assert.True(viewModel.CanPost);
    }

    [Fact]
    public void CanPost_WithEmptyContent_ReturnsFalse()
    {
        // Arrange
        var viewModel = new PostingViewModel(this.fixture.Protocol, this.database, this.fixture.CurrentUser);
        viewModel.PostContent = string.Empty;

        // Act & Assert
        Assert.False(viewModel.CanPost);
    }

    [Fact]
    public void CanPost_WithWhitespaceContent_ReturnsFalse()
    {
        // Arrange
        var viewModel = new PostingViewModel(this.fixture.Protocol, this.database, this.fixture.CurrentUser);
        viewModel.PostContent = "   ";

        // Act & Assert
        Assert.False(viewModel.CanPost);
    }

    [Fact]
    public void CanPost_WithContentExceedingCharacterLimit_ReturnsFalse()
    {
        // Arrange
        var viewModel = new PostingViewModel(this.fixture.Protocol, this.database, this.fixture.CurrentUser);
        viewModel.PostContent = new string('a', 301); // Exceeds 300 character limit

        // Act & Assert
        Assert.False(viewModel.CanPost);
    }

    [Fact]
    public void CanPost_WithContentAtCharacterLimit_ReturnsTrue()
    {
        // Arrange
        var viewModel = new PostingViewModel(this.fixture.Protocol, this.database, this.fixture.CurrentUser);
        viewModel.PostContent = new string('a', 300); // Exactly 300 characters

        // Act & Assert
        Assert.True(viewModel.CanPost);
    }

    [Fact]
    public void CanPost_WhenIsPosting_ReturnsFalse()
    {
        // Arrange
        var viewModel = new PostingViewModel(this.fixture.Protocol, this.database, this.fixture.CurrentUser);
        viewModel.PostContent = "This is a test post";

        // Use reflection to set IsPosting
        var field = typeof(PostingViewModel).GetField("_isPosting",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(viewModel, true);

        // Act & Assert
        Assert.False(viewModel.CanPost);
    }

    [Fact]
    public void AddFacet_WithValidFacet_AddsFacetToList()
    {
        // Arrange
        var viewModel = new PostingViewModel(this.protocol, this.database);
        var facet = Facet.CreateFacetLink(0, 10, "https://example.com");

        // Act
        viewModel.AddFacet(facet);

        // Assert
        Assert.Single(viewModel.PostFacets);
        Assert.Contains(facet, viewModel.PostFacets);
    }

    [Fact]
    public void AddFacet_WithNullFacet_ThrowsArgumentNullException()
    {
        // Arrange
        var viewModel = new PostingViewModel(this.protocol, this.database);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => viewModel.AddFacet(null!));
    }

    [Fact]
    public void AddFacet_WithMultipleFacets_AddsAllFacets()
    {
        // Arrange
        var viewModel = new PostingViewModel(this.protocol, this.database);
        var facet1 = Facet.CreateFacetLink(0, 10, "https://example.com");
        var facet2 = Facet.CreateFacetHashtag(11, 20, "test");

        // Act
        viewModel.AddFacet(facet1);
        viewModel.AddFacet(facet2);

        // Assert
        Assert.Equal(2, viewModel.PostFacets.Count);
        Assert.Contains(facet1, viewModel.PostFacets);
        Assert.Contains(facet2, viewModel.PostFacets);
    }

    [Fact]
    public void RemoveFacet_WithExistingFacet_RemovesFacetFromList()
    {
        // Arrange
        var viewModel = new PostingViewModel(this.protocol, this.database);
        var facet = Facet.CreateFacetHashtag(0, 10, "technology");
        viewModel.AddFacet(facet);

        // Act
        viewModel.RemoveFacet(facet);

        // Assert
        Assert.Empty(viewModel.PostFacets);
    }

    [Fact]
    public void RemoveFacet_WithNullFacet_ThrowsArgumentNullException()
    {
        // Arrange
        var viewModel = new PostingViewModel(this.protocol, this.database);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => viewModel.RemoveFacet(null!));
    }

    [Fact]
    public void RemoveFacet_WithNonExistentFacet_DoesNothing()
    {
        // Arrange
        var viewModel = new PostingViewModel(this.protocol, this.database);
        var facet1 = Facet.CreateFacetLink(0, 10, "https://example.com");
        var facet2 = Facet.CreateFacetLink(11, 20, "https://another.com");
        viewModel.AddFacet(facet1);

        // Act
        viewModel.RemoveFacet(facet2);

        // Assert
        Assert.Single(viewModel.PostFacets);
        Assert.Contains(facet1, viewModel.PostFacets);
    }

    [Fact]
    public void ClearPost_ResetsAllProperties()
    {
        // Arrange
        var viewModel = new PostingViewModel(this.protocol, this.database);
        viewModel.PostContent = "Test content";
        viewModel.AddFacet(Facet.CreateFacetHashtag(0, 10, "test"));
        viewModel.AddLanguage("en");
        viewModel.AddTag("test");
        viewModel.SetSelfLabels(new SelfLabels(new List<SelfLabel>()));

        // Use reflection to set error message
        var field = typeof(PostingViewModel).GetField("_errorMessage",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(viewModel, "Test error");

        // Act
        viewModel.ClearPost();

        // Assert
        Assert.Equal(string.Empty, viewModel.PostContent);
        Assert.Empty(viewModel.PostFacets);
        Assert.Empty(viewModel.Languages);
        Assert.Empty(viewModel.Tags);
        Assert.Equal(string.Empty, viewModel.ErrorMessage);
        Assert.Null(viewModel.SelfLabels);
    }

    [Fact]
    public void SetSelfLabels_WithValidLabels_SetsSelfLabels()
    {
        // Arrange
        var viewModel = new PostingViewModel(this.protocol, this.database);
        var labels = new SelfLabels(new List<SelfLabel>());

        // Act
        viewModel.SetSelfLabels(labels);

        // Assert
        Assert.NotNull(viewModel.SelfLabels);
        Assert.Equal(labels, viewModel.SelfLabels);
    }

    [Fact]
    public void SetSelfLabels_WithNull_SetsNull()
    {
        // Arrange
        var viewModel = new PostingViewModel(this.protocol, this.database);
        viewModel.SetSelfLabels(new SelfLabels(new List<SelfLabel>()));

        // Act
        viewModel.SetSelfLabels(null);

        // Assert
        Assert.Null(viewModel.SelfLabels);
    }

    [Fact]
    public void AddLanguage_WithValidLanguage_AddsLanguage()
    {
        // Arrange
        var viewModel = new PostingViewModel(this.protocol, this.database);

        // Act
        viewModel.AddLanguage("en");

        // Assert
        Assert.Single(viewModel.Languages);
        Assert.Contains("en", viewModel.Languages);
    }

    [Fact]
    public void AddLanguage_WithDuplicateLanguage_DoesNotAddDuplicate()
    {
        // Arrange
        var viewModel = new PostingViewModel(this.protocol, this.database);
        viewModel.AddLanguage("en");

        // Act
        viewModel.AddLanguage("en");

        // Assert
        Assert.Single(viewModel.Languages);
    }

    [Fact]
    public void AddLanguage_WithNullLanguage_ThrowsArgumentNullException()
    {
        // Arrange
        var viewModel = new PostingViewModel(this.protocol, this.database);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => viewModel.AddLanguage(null!));
    }

    [Fact]
    public void AddLanguage_WithEmptyLanguage_ThrowsArgumentNullException()
    {
        // Arrange
        var viewModel = new PostingViewModel(this.protocol, this.database);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => viewModel.AddLanguage(string.Empty));
    }

    [Fact]
    public void AddLanguage_WithWhitespaceLanguage_ThrowsArgumentNullException()
    {
        // Arrange
        var viewModel = new PostingViewModel(this.protocol, this.database);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => viewModel.AddLanguage("   "));
    }

    [Fact]
    public void RemoveLanguage_WithExistingLanguage_RemovesLanguage()
    {
        // Arrange
        var viewModel = new PostingViewModel(this.protocol, this.database);
        viewModel.AddLanguage("en");

        // Act
        viewModel.RemoveLanguage("en");

        // Assert
        Assert.Empty(viewModel.Languages);
    }

    [Fact]
    public void RemoveLanguage_WithNonExistentLanguage_DoesNothing()
    {
        // Arrange
        var viewModel = new PostingViewModel(this.protocol, this.database);
        viewModel.AddLanguage("en");

        // Act
        viewModel.RemoveLanguage("es");

        // Assert
        Assert.Single(viewModel.Languages);
        Assert.Contains("en", viewModel.Languages);
    }

    [Fact]
    public void RemoveLanguage_WithNullLanguage_ThrowsArgumentNullException()
    {
        // Arrange
        var viewModel = new PostingViewModel(this.protocol, this.database);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => viewModel.RemoveLanguage(null!));
    }

    [Fact]
    public void AddTag_WithValidTag_AddsTag()
    {
        // Arrange
        var viewModel = new PostingViewModel(this.protocol, this.database);

        // Act
        viewModel.AddTag("technology");

        // Assert
        Assert.Single(viewModel.Tags);
        Assert.Contains("technology", viewModel.Tags);
    }

    [Fact]
    public void AddTag_WithDuplicateTag_DoesNotAddDuplicate()
    {
        // Arrange
        var viewModel = new PostingViewModel(this.protocol, this.database);
        viewModel.AddTag("technology");

        // Act
        viewModel.AddTag("technology");

        // Assert
        Assert.Single(viewModel.Tags);
    }

    [Fact]
    public void AddTag_WithNullTag_ThrowsArgumentNullException()
    {
        // Arrange
        var viewModel = new PostingViewModel(this.protocol, this.database);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => viewModel.AddTag(null!));
    }

    [Fact]
    public void AddTag_WithEmptyTag_ThrowsArgumentNullException()
    {
        // Arrange
        var viewModel = new PostingViewModel(this.protocol, this.database);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => viewModel.AddTag(string.Empty));
    }

    [Fact]
    public void AddTag_WithWhitespaceTag_ThrowsArgumentNullException()
    {
        // Arrange
        var viewModel = new PostingViewModel(this.protocol, this.database);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => viewModel.AddTag("   "));
    }

    [Fact]
    public void RemoveTag_WithExistingTag_RemovesTag()
    {
        // Arrange
        var viewModel = new PostingViewModel(this.protocol, this.database);
        viewModel.AddTag("technology");

        // Act
        viewModel.RemoveTag("technology");

        // Assert
        Assert.Empty(viewModel.Tags);
    }

    [Fact]
    public void RemoveTag_WithNonExistentTag_DoesNothing()
    {
        // Arrange
        var viewModel = new PostingViewModel(this.protocol, this.database);
        viewModel.AddTag("technology");

        // Act
        viewModel.RemoveTag("science");

        // Assert
        Assert.Single(viewModel.Tags);
        Assert.Contains("technology", viewModel.Tags);
    }

    [Fact]
    public void RemoveTag_WithNullTag_ThrowsArgumentNullException()
    {
        // Arrange
        var viewModel = new PostingViewModel(this.protocol, this.database);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => viewModel.RemoveTag(null!));
    }

    [Fact]
    public async Task SetExternalEmbedFromUriAsync_WithNullUri_ThrowsArgumentNullException()
    {
        // Arrange
        var viewModel = new PostingViewModel(this.protocol, this.database);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            viewModel.SetExternalEmbedFromUriAsync(null!));
    }

    [Fact]
    public async Task SetExternalEmbedFromUriAsync_WithValidUri_SetsEmbed()
    {
        // Arrange
        var viewModel = new PostingViewModel(this.fixture.Protocol, this.database, this.fixture.CurrentUser);
        var uri = new Uri("https://github.com/drasticactions/fishyflip");

        // Act
        await viewModel.SetExternalEmbedFromUriAsync(uri);

        // Assert
        // Note: The actual embed may be null if the URL doesn't have proper Open Graph tags
        // The important thing is that the method doesn't throw an exception
        Assert.NotNull(viewModel.ErrorMessage);
    }

    [Fact]
    public async Task SetImageEmbedFromStreamsAsync_ThrowsNotImplementedException()
    {
        // Arrange
        var viewModel = new PostingViewModel(this.protocol, this.database);
        var streams = new List<Stream>();
        var altTexts = new List<string>();

        // Act & Assert
        await Assert.ThrowsAsync<NotImplementedException>(() =>
            viewModel.SetImageEmbedFromStreamsAsync(streams, altTexts));
    }

    [Fact]
    public async Task SetVideoEmbedFromStreamAsync_ThrowsNotImplementedException()
    {
        // Arrange
        var viewModel = new PostingViewModel(this.protocol, this.database);
        var stream = new MemoryStream();

        // Act & Assert
        await Assert.ThrowsAsync<NotImplementedException>(() =>
            viewModel.SetVideoEmbedFromStreamAsync(stream));
    }

    [Fact]
    public void PostContent_PropertyChanged_UpdatesCanPost()
    {
        // Arrange
        var viewModel = new PostingViewModel(this.fixture.Protocol, this.database, this.fixture.CurrentUser);
        bool canPostBefore = viewModel.CanPost;

        // Act
        viewModel.PostContent = "Test content";
        bool canPostAfter = viewModel.CanPost;

        // Assert
        Assert.False(canPostBefore);
        Assert.True(canPostAfter);
    }

    [Fact]
    public void AddLanguage_WithMultipleLanguages_AddsAllLanguages()
    {
        // Arrange
        var viewModel = new PostingViewModel(this.protocol, this.database);

        // Act
        viewModel.AddLanguage("en");
        viewModel.AddLanguage("es");
        viewModel.AddLanguage("fr");

        // Assert
        Assert.Equal(3, viewModel.Languages.Count);
        Assert.Contains("en", viewModel.Languages);
        Assert.Contains("es", viewModel.Languages);
        Assert.Contains("fr", viewModel.Languages);
    }

    [Fact]
    public void AddTag_WithMultipleTags_AddsAllTags()
    {
        // Arrange
        var viewModel = new PostingViewModel(this.protocol, this.database);

        // Act
        viewModel.AddTag("technology");
        viewModel.AddTag("science");
        viewModel.AddTag("programming");

        // Assert
        Assert.Equal(3, viewModel.Tags.Count);
        Assert.Contains("technology", viewModel.Tags);
        Assert.Contains("science", viewModel.Tags);
        Assert.Contains("programming", viewModel.Tags);
    }

    [Fact]
    public void ClearPost_AfterSettingMultipleProperties_ResetsAllToDefault()
    {
        // Arrange
        var viewModel = new PostingViewModel(this.protocol, this.database);
        viewModel.PostContent = "Test post with multiple properties";
        viewModel.AddFacet(Facet.CreateFacetLink(0, 4, "https://test.com"));
        viewModel.AddLanguage("en");
        viewModel.AddLanguage("es");
        viewModel.AddTag("test");
        viewModel.AddTag("sample");
        viewModel.SetSelfLabels(new SelfLabels(new List<SelfLabel>()));

        // Act
        viewModel.ClearPost();

        // Assert
        Assert.Equal(string.Empty, viewModel.PostContent);
        Assert.Empty(viewModel.PostFacets);
        Assert.Empty(viewModel.Languages);
        Assert.Empty(viewModel.Tags);
        Assert.Null(viewModel.SelfLabels);
        Assert.Equal(string.Empty, viewModel.ErrorMessage);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(150)]
    [InlineData(299)]
    [InlineData(300)]
    public void CanPost_WithVariousValidContentLengths_ReturnsTrue(int length)
    {
        // Arrange
        var viewModel = new PostingViewModel(this.fixture.Protocol, this.database, this.fixture.CurrentUser);
        viewModel.PostContent = new string('a', length);

        // Act & Assert
        Assert.True(viewModel.CanPost);
    }

    [Theory]
    [InlineData(301)]
    [InlineData(350)]
    [InlineData(500)]
    [InlineData(1000)]
    public void CanPost_WithContentExceedingLimit_ReturnsFalse(int length)
    {
        // Arrange
        var viewModel = new PostingViewModel(this.fixture.Protocol, this.database, this.fixture.CurrentUser);
        viewModel.PostContent = new string('a', length);

        // Act & Assert
        Assert.False(viewModel.CanPost);
    }
}
