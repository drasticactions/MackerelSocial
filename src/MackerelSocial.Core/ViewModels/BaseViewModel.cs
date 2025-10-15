using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using FishyFlip;
using MackerelSocial.Core.Events;
using MackerelSocial.Core.Models;
using MackerelSocial.Core.Services;

namespace MackerelSocial.Core.ViewModels;

public abstract partial class BaseViewModel : ObservableObject, IDisposable
{
    public BaseViewModel(ATProtocol protocol, DatabaseService database)
    {
        if (protocol == null)
        {
            throw new ArgumentNullException(nameof(protocol));
        }

        if (database == null)
        {
            throw new ArgumentNullException(nameof(database));
        }
        
        this.Protocol = protocol;
        this.Database = database;
        StrongReferenceMessenger.Default.Register<OnLoginUserEventArgs>(this, this.OnLoginUser);
    }

    /// <summary>
    /// Gets the <see cref="ATProtocol"/> instance.
    /// </summary>
    public ATProtocol Protocol { get; }

    public DatabaseService Database { get; }

    [ObservableProperty]
    private LoginUser? _currentUser;

    public bool IsAuthenticated => this.CurrentUser is not null;

    protected virtual void OnLoginUser(object recipient, OnLoginUserEventArgs args)
    {
        this.CurrentUser = args.LoginUser;
    }

    /// <summary>
    /// Dispose the view model.
    /// </summary>
    public virtual void Dispose()
    {
        StrongReferenceMessenger.Default.Unregister<OnLoginUserEventArgs>(this);
    }
}