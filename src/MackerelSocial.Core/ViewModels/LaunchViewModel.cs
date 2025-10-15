using System.Text.Json;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FishyFlip;
using FishyFlip.Models;
using MackerelSocial.Core;
using MackerelSocial.Events;
using MackerelSocial.Services;

namespace MackerelSocial.ViewModels;

public partial class LaunchViewModel : BaseViewModel
{
    public LaunchViewModel(ATProtocol protocol, DatabaseService database)
        : base(protocol, database)
    {
        this.DiscoverFeed = new FeedViewCollection(protocol, new ATUri("at://did:plc:z72i7hdynmk6r22z27h6tvur/app.bsky.feed.generator/whats-hot"));
    }

    [RelayCommand]
    public async Task LaunchLoginAsync()
    {
        StrongReferenceMessenger.Default.Send(new ShowViewModelEventArgs(ShowViewModel.Login));
    }

    public FeedViewCollection DiscoverFeed { get; }
}