using CommunityToolkit.Mvvm.Messaging;
using MackerelSocial.Core.Events;
using MackerelSocial.Core.Models;
using SQLite;

namespace MackerelSocial.Core.Services;

/// <summary>
/// Database Service.
/// </summary>
public class DatabaseService : IDisposable
{
    private const SQLite.SQLiteOpenFlags Flags =
        SQLite.SQLiteOpenFlags.ReadWrite |
        SQLite.SQLiteOpenFlags.Create |
        SQLite.SQLiteOpenFlags.SharedCache;

    private SQLiteAsyncConnection database;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseService"/> class.
    /// </summary>
    /// <param name="connectionString">Connection String.</param>
    public DatabaseService(string connectionString)
    {
        SQLitePCL.Batteries.Init();
        this.database = new SQLiteAsyncConnection(connectionString, Flags);
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="DatabaseService"/> class.
    /// </summary>
    ~DatabaseService()
    {
        this.ReleaseUnmanagedResources();
    }

    public async Task InitializeAsync()
    {
        await this.database.CreateTablesAsync<Models.LoginUser, Models.AppSettings>().ConfigureAwait(false);
    }

    public async Task<IEnumerable<Models.LoginUser>> GetLoginUsersAsync()
    {
        return await this.database.Table<Models.LoginUser>().ToListAsync().ConfigureAwait(false);
    }

    public async Task<Models.LoginUser?> GetLoginUserAsync(int id)
    {
        return await this.database.Table<Models.LoginUser>().Where(i => i.Id == id).FirstOrDefaultAsync().ConfigureAwait(false);
    }

    public async Task<Models.LoginUser?> GetDefaultLoginUserAsync()
    {
        return await this.database.Table<Models.LoginUser>().Where(i => i.IsDefault).FirstOrDefaultAsync().ConfigureAwait(false);
    }

    public async Task<Models.LoginUser?> GetLoginUserByDidAsync(string did)
    {
        return await this.database.Table<Models.LoginUser>().Where(i => i.Did == did).FirstOrDefaultAsync().ConfigureAwait(false);
    }

    public async Task<Models.LoginUser?> GetLoginUserByHandleAsync(string handle)
    {
        return await this.database.Table<Models.LoginUser>().Where(i => i.Handle == handle).FirstOrDefaultAsync().ConfigureAwait(false);
    }

    public async Task<int> SaveLoginUserAsync(Models.LoginUser item)
    {
        int result = 0;
        if (item.Id != 0)
        {
            result = await this.database.UpdateAsync(item).ConfigureAwait(false);
            if (result > 0)
            {
                StrongReferenceMessenger.Default.Send(new OnLoginUserEventArgs(item, Models.DatabaseEvent.Updated));
            }
        }
        else
        {
            result = await this.database.InsertAsync(item).ConfigureAwait(false);
            if (result > 0)
            {
                StrongReferenceMessenger.Default.Send(new OnLoginUserEventArgs(item, Models.DatabaseEvent.Inserted));
            }
        }
        return result;
    }

    public async Task<int> SaveOrUpdateLoginUserAsDefaultAsync(Models.LoginUser item)
    {
        // First, find if this user already exists (by DID or Handle)
        Models.LoginUser? existingUser = null;

        if (!string.IsNullOrEmpty(item.Did))
        {
            existingUser = await GetLoginUserByDidAsync(item.Did).ConfigureAwait(false);
        }

        if (existingUser == null && !string.IsNullOrEmpty(item.Handle))
        {
            existingUser = await GetLoginUserByHandleAsync(item.Handle).ConfigureAwait(false);
        }

        // If user exists, update their information
        if (existingUser != null)
        {
            existingUser.Handle = item.Handle;
            existingUser.Email = item.Email;
            existingUser.Did = item.Did;
            existingUser.SessionData = item.SessionData;
            existingUser.LoginType = item.LoginType;
            existingUser.IsDefault = true; // This will be the default account
            item = existingUser; // Use the existing user with updated data
        }
        else
        {
            // New user, set as default
            item.IsDefault = true;
        }

        // Unset all other users as non-default
        await UnsetAllDefaultUsersAsync().ConfigureAwait(false);

        // Save the user (insert or update)
        return await SaveLoginUserAsync(item).ConfigureAwait(false);
    }

    private async Task UnsetAllDefaultUsersAsync()
    {
        var defaultUsers = await this.database.Table<Models.LoginUser>().Where(u => u.IsDefault).ToListAsync().ConfigureAwait(false);
        foreach (var user in defaultUsers)
        {
            user.IsDefault = false;
            await this.database.UpdateAsync(user).ConfigureAwait(false);
        }
    }

    public async Task<int> DeleteLoginUserAsync(Models.LoginUser item)
    {
        var result = await this.database.DeleteAsync(item).ConfigureAwait(false);
        if (result > 0)
        {
            StrongReferenceMessenger.Default.Send(new OnLoginUserEventArgs(item, Models.DatabaseEvent.Deleted));
        }

        return result;
    }

    /// <summary>
    /// Get AppSettings.
    /// </summary>
    /// <returns><see cref="AppSettings"/>.</returns>
    public async Task<AppSettings?> GetAppSettingsAsync()
    {
        try
        {
            var settings = await this.database.Table<AppSettings>().FirstOrDefaultAsync();
            if (settings == null)
            {
                settings = new AppSettings();
                await this.database.InsertAsync(settings, typeof(AppSettings));
            }

            return settings;
        }
        catch (Exception ex)
        {
            StrongReferenceMessenger.Default.Send(new OnExceptionEventArgs("Failed to get AppSettings from database.", ex));
            return null;
        }
    }

    /// <summary>
    /// Update AppSettings.
    /// </summary>
    /// <param name="settings"><see cref="AppSettings"/>.</param>
    /// <returns>If the app settings were updated.</returns>
    public async Task<bool> SaveAppSettingsAsync(AppSettings settings)
    {
        int rows = 0;

        try
        {
            rows = await this.database.UpdateAsync(settings, typeof(AppSettings));
        }
        catch (Exception ex)
        {
            StrongReferenceMessenger.Default.Send(new OnExceptionEventArgs("Failed to update AppSettings in database.", ex));
            return false;
        }

        return rows > 0;
    }

    /// <summary>
    /// Dispose elements.
    /// </summary>
    public void Dispose()
    {
        this.ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources()
    {
        this.database.CloseAsync();
    }
}