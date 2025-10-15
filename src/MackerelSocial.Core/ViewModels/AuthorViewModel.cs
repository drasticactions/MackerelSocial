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

public partial class AuthorViewModel : BaseViewModel
{
    public AuthorViewModel(ATIdentifier identifier, ATProtocol protocol, DatabaseService database, string filter = "")
        : base(protocol, database)
    {
        this.AuthorFeed = new AuthorViewCollection(protocol, identifier, filter);
    }
    
    public AuthorViewCollection AuthorFeed { get; }
}