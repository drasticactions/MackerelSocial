using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FishyFlip;
using FishyFlip.Lexicon;
using FishyFlip.Lexicon.App.Bsky.Embed;
using FishyFlip.Lexicon.App.Bsky.Richtext;
using FishyFlip.Lexicon.Com.Atproto.Label;
using FishyFlip.Models;
using FishyFlip.Tools;
using MackerelSocial.Core;
using MackerelSocial.Core.Events;
using MackerelSocial.Core.Services;
using MackerelSocial.Core.Utilities;
using Microsoft.Extensions.Logging;
using ZstdSharp.Unsafe;

namespace MackerelSocial.Core.ViewModels;

/// <summary>
/// View model for making posts.
/// </summary>
public partial class PostingViewModel : BaseViewModel
{
    private int characterLimit = 300;

    private FileContentTypeDetector fileContentTypeDetector;

    public PostingViewModel(ATProtocol protocol, DatabaseService database, ILogger? logger = null)
        : base(protocol, database, logger)
    {
        fileContentTypeDetector = new FileContentTypeDetector(this.Logger);
    }

    [ObservableProperty]
    private string _postContent = string.Empty;

    [ObservableProperty]
    private List<Facet> _postFacets = new List<Facet>();

    [ObservableProperty]
    private bool _isPosting = false;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private SelfLabels? _selfLabels;

    [ObservableProperty]
    private ATObject? _embed;

    [ObservableProperty]
    private List<string> _languages = new List<string> { };

    [ObservableProperty]
    private List<string> _tags = new List<string> { };
    public bool CanPost => this.CurrentUser != null && !string.IsNullOrWhiteSpace(this.PostContent) && !this.IsPosting && this.PostContent.Length <= this.characterLimit;

    [RelayCommand(CanExecute = nameof(CanPost))]
    private async Task PostAsync(CancellationToken cancellationToken = default)
    {
        if (this.CurrentUser == null)
        {
            return;
        }

        try
        {
            this.IsPosting = true;
            var (result, error) = await this.Protocol.Feed.CreatePostAsync(text: this.PostContent, facets: this.PostFacets, embed: this.Embed, tags: this.Tags, langs: this.Languages, labels: this.SelfLabels, cancellationToken: cancellationToken);
            if (error != null)
            {
                this.ErrorMessage = $"Failed to create post: {error.Detail}";
                return;
            }
        }
        catch (Exception ex)
        {
            this.ErrorMessage = $"Failed to create post: {ex.Message}";
        }
        finally
        {
            this.IsPosting = false;
        }
    }

    public void AddFacet(Facet facet)
    {
        if (facet == null)
        {
            throw new ArgumentNullException(nameof(facet));
        }

        this.PostFacets.Add(facet);
        this.OnPropertyChanged(nameof(this.PostFacets));
        this.OnPropertyChanged(nameof(this.CanPost));
    }

    public void RemoveFacet(Facet facet)
    {
        if (facet == null)
        {
            throw new ArgumentNullException(nameof(facet));
        }

        this.PostFacets.Remove(facet);
        this.OnPropertyChanged(nameof(this.PostFacets));
        this.OnPropertyChanged(nameof(this.CanPost));
    }

    public void ClearPost()
    {
        this.PostContent = string.Empty;
        this.PostFacets.Clear();
        this.Languages.Clear();
        this.Tags.Clear();
        this.ErrorMessage = string.Empty;
        this.SelfLabels = null;
        this.OnPropertyChanged(nameof(this.PostFacets));
        this.OnPropertyChanged(nameof(this.CanPost));
    }

    public void SetSelfLabels(SelfLabels? labels)
    {
        this.SelfLabels = labels;
        this.OnPropertyChanged(nameof(this.SelfLabels));
    }

    public void AddLanguage(string language)
    {
        if (string.IsNullOrWhiteSpace(language))
        {
            throw new ArgumentNullException(nameof(language));
        }

        if (!this.Languages.Contains(language))
        {
            this.Languages.Add(language);
            this.OnPropertyChanged(nameof(this.Languages));
        }
    }

    public void RemoveLanguage(string language)
    {
        if (string.IsNullOrWhiteSpace(language))
        {
            throw new ArgumentNullException(nameof(language));
        }

        if (this.Languages.Contains(language))
        {
            this.Languages.Remove(language);
            this.OnPropertyChanged(nameof(this.Languages));
        }
    }

    public void AddTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
        {
            throw new ArgumentNullException(nameof(tag));
        }

        if (!this.Tags.Contains(tag))
        {
            this.Tags.Add(tag);
            this.OnPropertyChanged(nameof(this.Tags));
        }
    }

    public void RemoveTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
        {
            throw new ArgumentNullException(nameof(tag));
        }

        if (this.Tags.Contains(tag))
        {
            this.Tags.Remove(tag);
            this.OnPropertyChanged(nameof(this.Tags));
        }
    }

    public async Task SetImageEmbedFromStreamsAsync(List<Stream> imageStreams, List<string> altTexts, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task SetVideoEmbedFromStreamAsync(Stream videoStream, string? altText = default, Caption? caption = default, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task SetExternalEmbedFromUriAsync(Uri uri, CancellationToken cancellationToken = default)
    {
        if (uri == null)
        {
            throw new ArgumentNullException(nameof(uri));
        }

        try
        {
            var embed = await this.Protocol.OpenGraphParser.GenerateEmbedExternal(uri.ToString());
            if (embed == null)
            {
                this.ErrorMessage = $"Failed to create external embed";
                return;
            }

            this.Embed = embed;
            this.OnPropertyChanged(nameof(this.Embed));
            this.OnPropertyChanged(nameof(this.CanPost));
        }
        catch (Exception ex)
        {
            this.ErrorMessage = $"Failed to create external embed: {ex.Message}";
        }
    }
}