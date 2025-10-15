using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FishyFlip;
using FishyFlip.Lexicon;
using FishyFlip.Models;
using FishyFlip.Tools;
using MackerelSocial.Core;
using MackerelSocial.Core.Events;
using MackerelSocial.Core.Services;
using Microsoft.Extensions.Logging;

namespace MackerelSocial.Core.ViewModels;

/// <summary>
/// View model for repository-related data and actions.
/// </summary>
public partial class RepoViewModel : BaseViewModel
{
    public RepoViewModel(ATProtocol protocol, DatabaseService database, ILogger? logger = null)
        : base(protocol, database, logger)
    {
    }

    /// <summary>
    /// Gets or sets the selected ATObject.
    /// </summary>
    [ObservableProperty]
    private ATObject? selectedATObject;

    /// <summary>
    /// Gets or sets a value indicating whether the ViewModel is busy.
    /// </summary>
    [ObservableProperty]
    private bool isBusy = false;

    /// <summary>
    /// Gets the collection of ATObjects.
    /// </summary>
    public ObservableCollection<ATObject> ATObjects { get; } = new();

    public async Task OpenRepoStreamAsync(Stream stream, CancellationToken token)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        token.ThrowIfCancellationRequested();

        try
        {
            this.IsBusy = true;
            this.ATObjects.Clear();

            var repoFile = CarDecoder.DecodeRepoAsync(stream);
            await foreach (var item in repoFile)
            {
                this.ATObjects.Add(item);
            }
        }
        catch (Exception ex)
        {
            StrongReferenceMessenger.Default.Send(new OnExceptionEventArgs("Failed to open repo stream.", ex));
        }
        finally
        {
            this.IsBusy = false;
        }
    }

    public async Task OpenRepoFromIdentifierAsync(ATIdentifier identifier, CancellationToken token)
    {
        if (identifier == null)
        {
            throw new ArgumentNullException(nameof(identifier));
        }

        token.ThrowIfCancellationRequested();

        try
        {
            this.IsBusy = true;
            this.ATObjects.Clear();

            using var memoryStream = new MemoryStream();
            var (result, error) = await this.Protocol.DownloadRepoAsync(identifier, memoryStream, cancellationToken: token);
            if (error != null)
            {
                StrongReferenceMessenger.Default.Send(new OnATErrorEventArgs("Failed to download repo.", error));
            }
            else
            {
                memoryStream.Seek(0, SeekOrigin.Begin);
                await this.OpenRepoStreamAsync(memoryStream, token);
            }
        }
        catch (Exception ex)
        {
            StrongReferenceMessenger.Default.Send(new OnExceptionEventArgs("Failed to open repo from identifier.", ex));
        }
        finally
        {
            this.IsBusy = false;
        }
    }
}