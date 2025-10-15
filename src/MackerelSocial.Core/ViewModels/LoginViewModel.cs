using System.Text.Json;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FishyFlip;
using FishyFlip.Models;
using MackerelSocial.Core.Events;
using MackerelSocial.Core.Services;
using Microsoft.Extensions.Logging;

namespace MackerelSocial.Core.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private static readonly Regex EmailRegex = new(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled);

    public LoginViewModel(ATProtocol protocol, DatabaseService database, ILogger? logger = null)
        : base(protocol, database, logger)
    {
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginWithPasswordCommand))]
    [NotifyPropertyChangedFor(nameof(CanLoginWithPassword))]
    [NotifyPropertyChangedFor(nameof(CanLoginWithOauth))]
    private string _identifier = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginWithPasswordCommand))]
    [NotifyPropertyChangedFor(nameof(CanLoginWithPassword))]
    private string _password = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginWithPasswordCommand))]
    [NotifyPropertyChangedFor(nameof(CanLoginWithPassword))]
    [NotifyPropertyChangedFor(nameof(CanLoginWithOauth))]
    private bool _isLoggingInWithPassword;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginWithOAuthCommand))]
    [NotifyPropertyChangedFor(nameof(CanLoginWithPassword))]
    [NotifyPropertyChangedFor(nameof(CanLoginWithOauth))]
    private bool _isLoggingInWithOAuth;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    /// <summary>
    /// Gets a value indicating whether any login operation is in progress.
    /// </summary>
    public bool IsLoggingIn => IsLoggingInWithPassword || IsLoggingInWithOAuth;

    /// <summary>
    /// Gets a value indicating whether the user can log in with password.
    /// </summary>
    public bool CanLoginWithPassword => IsValidIdentifier() && !string.IsNullOrWhiteSpace(this.Password) && !this.IsLoggingIn;

    /// <summary>
    /// Gets a value indicating whether the user can log in with OAuth.
    /// </summary>
    public bool CanLoginWithOauth => IsValidOAuthIdentifier() && !this.IsLoggingIn;

    [RelayCommand(CanExecute = nameof(CanLoginWithPassword))]
    public async Task<bool> LoginWithPasswordAsync()
    {
        try
        {
            this.IsLoggingInWithPassword = true;
            this.ErrorMessage = string.Empty;
            var (result, error) = await this.Protocol.AuthenticateWithPasswordResultAsync(this.Identifier, this.Password);
            if (error != null)
            {
                this.ErrorMessage = error.Detail?.Message ?? "An unknown error occurred.";
                return false;
            }

            if (result != null)
            {
                var loginUser = new Models.LoginUser
                {
                    Handle = result.Handle.Handle,
                    Email = result.Email ?? string.Empty,
                    Did = result.Did.ToString(),
                    SessionData = JsonSerializer.Serialize<Session>(result, this.Protocol.Options.JsonSerializerOptions),
                    LoginType = Models.LoginType.Password
                };

                await this.Database.SaveOrUpdateLoginUserAsDefaultAsync(loginUser);
            }

            return true;
        }
        catch (Exception ex)
        {
            this.ErrorMessage = ex.Message;
            return false;
        }
        finally
        {
            this.IsLoggingInWithPassword = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanLoginWithOauth))]
    public async Task<bool> LoginWithOAuthAsync()
    {
        try
        {
            this.IsLoggingInWithOAuth = true;
            this.ErrorMessage = string.Empty;

            // TODO: Implement OAuth login flow
            await Task.Delay(5000);

            return false;
        }
        catch (Exception ex)
        {
            this.ErrorMessage = ex.Message;
            return false;
        }
        finally
        {
            this.IsLoggingInWithOAuth = false;
        }
    }

    private bool IsValidIdentifier()
    {
        if (string.IsNullOrWhiteSpace(this.Identifier))
        {
            return false;
        }

        // Check if it's a valid email address
        if (EmailRegex.IsMatch(this.Identifier))
        {
            return true;
        }

        // Check if it's a valid AT Protocol identifier (handle or DID)
        return ATIdentifier.TryCreate(this.Identifier, out _);
    }

    private bool IsValidOAuthIdentifier()
    {
        if (string.IsNullOrWhiteSpace(this.Identifier))
        {
            return false;
        }

        // OAuth requires AT Protocol identifier (handle or DID), not email
        // Reject if it's an email address
        if (EmailRegex.IsMatch(this.Identifier))
        {
            return false;
        }

        // Check if it's a valid AT Protocol identifier (handle or DID)
        return ATIdentifier.TryCreate(this.Identifier, out _);
    }

    protected override void OnLoginUser(object recipient, OnLoginUserEventArgs args)
    {
        base.OnLoginUser(recipient, args);

        // Clear data on login
        this.Password = string.Empty;
        this.IsLoggingInWithPassword = false;
        this.IsLoggingInWithOAuth = false;
        this.ErrorMessage = string.Empty;
        this.Identifier = string.Empty;
    }
}