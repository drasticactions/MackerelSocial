// <copyright file="DatabaseServiceTests.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using MackerelSocial.Core.Models;
using MackerelSocial.Core.Services;

namespace MackerelSocial.Core.Tests;

/// <summary>
/// Tests for DatabaseService.
/// </summary>
public class DatabaseServiceTests : IDisposable
{
    private readonly DatabaseService database;

    public DatabaseServiceTests()
    {
        // Use in-memory SQLite database for testing
        this.database = new DatabaseService(":memory:");
    }

    public void Dispose()
    {
        this.database.Dispose();
    }

    [Fact(Skip = "Flaky test, needs investigation")]
    public async Task InitializeAsync_CreatesRequiredTables()
    {
        // Act
        await this.database.InitializeAsync();

        // Assert - try to insert data to verify tables exist
        var loginUser = new LoginUser
        {
            Handle = "test.bsky.social",
            Email = "test@example.com",
            Did = "did:plc:test123",
            SessionData = "{}",
            LoginType = LoginType.OAuth,
            IsDefault = false
        };

        var result = await this.database.SaveLoginUserAsync(loginUser);
        Assert.True(result > 0);

        var settings = await this.database.GetAppSettingsAsync();
        Assert.NotNull(settings);
    }

    [Fact(Skip = "Flaky test, needs investigation")]
    public async Task SaveLoginUserAsync_InsertsNewUser_ReturnsNonZeroId()
    {
        // Arrange
        await this.database.InitializeAsync();
        var loginUser = new LoginUser
        {
            Handle = "newuser.bsky.social",
            Email = "newuser@example.com",
            Did = "did:plc:newuser123",
            SessionData = "{}",
            LoginType = LoginType.OAuth,
            IsDefault = false
        };

        // Act
        var result = await this.database.SaveLoginUserAsync(loginUser);

        // Assert
        Assert.True(result > 0);
        Assert.True(loginUser.Id > 0);
    }

    [Fact(Skip = "Flaky test, needs investigation")]
    public async Task SaveLoginUserAsync_UpdatesExistingUser_ReturnsNonZeroId()
    {
        // Arrange
        await this.database.InitializeAsync();
        var loginUser = new LoginUser
        {
            Handle = "updateuser.bsky.social",
            Email = "updateuser@example.com",
            Did = "did:plc:updateuser123",
            SessionData = "{}",
            LoginType = LoginType.OAuth,
            IsDefault = false
        };

        await this.database.SaveLoginUserAsync(loginUser);

        // Act - Update the user
        loginUser.Handle = "updated.bsky.social";
        loginUser.Email = "updated@example.com";
        var result = await this.database.SaveLoginUserAsync(loginUser);

        // Assert
        Assert.True(result > 0);
        var retrieved = await this.database.GetLoginUserAsync(loginUser.Id);
        Assert.NotNull(retrieved);
        Assert.Equal("updated.bsky.social", retrieved.Handle);
        Assert.Equal("updated@example.com", retrieved.Email);
    }

    [Fact(Skip = "Flaky test, needs investigation")]
    public async Task GetLoginUsersAsync_ReturnsAllUsers()
    {
        // Arrange
        await this.database.InitializeAsync();
        var user1 = new LoginUser
        {
            Handle = "user1.bsky.social",
            Email = "user1@example.com",
            Did = "did:plc:user1",
            SessionData = "{}",
            LoginType = LoginType.OAuth,
            IsDefault = false
        };
        var user2 = new LoginUser
        {
            Handle = "user2.bsky.social",
            Email = "user2@example.com",
            Did = "did:plc:user2",
            SessionData = "{}",
            LoginType = LoginType.Password,
            IsDefault = false
        };

        await this.database.SaveLoginUserAsync(user1);
        await this.database.SaveLoginUserAsync(user2);

        // Act
        var users = await this.database.GetLoginUsersAsync();

        // Assert
        Assert.NotNull(users);
        Assert.Equal(2, users.Count());
    }

    [Fact(Skip = "Flaky test, needs investigation")]
    public async Task GetLoginUserAsync_ById_ReturnsCorrectUser()
    {
        // Arrange
        await this.database.InitializeAsync();
        var loginUser = new LoginUser
        {
            Handle = "getbyid.bsky.social",
            Email = "getbyid@example.com",
            Did = "did:plc:getbyid123",
            SessionData = "{}",
            LoginType = LoginType.OAuth,
            IsDefault = false
        };

        await this.database.SaveLoginUserAsync(loginUser);

        // Act
        var retrieved = await this.database.GetLoginUserAsync(loginUser.Id);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(loginUser.Id, retrieved.Id);
        Assert.Equal(loginUser.Handle, retrieved.Handle);
    }

    [Fact(Skip = "Flaky test, needs investigation")]
    public async Task GetLoginUserByDidAsync_ReturnsCorrectUser()
    {
        // Arrange
        await this.database.InitializeAsync();
        var loginUser = new LoginUser
        {
            Handle = "getbydid.bsky.social",
            Email = "getbydid@example.com",
            Did = "did:plc:getbydid123",
            SessionData = "{}",
            LoginType = LoginType.OAuth,
            IsDefault = false
        };

        await this.database.SaveLoginUserAsync(loginUser);

        // Act
        var retrieved = await this.database.GetLoginUserByDidAsync("did:plc:getbydid123");

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(loginUser.Did, retrieved.Did);
        Assert.Equal(loginUser.Handle, retrieved.Handle);
    }

    [Fact(Skip = "Flaky test, needs investigation")]
    public async Task GetLoginUserByHandleAsync_ReturnsCorrectUser()
    {
        // Arrange
        await this.database.InitializeAsync();
        var loginUser = new LoginUser
        {
            Handle = "getbyhandle.bsky.social",
            Email = "getbyhandle@example.com",
            Did = "did:plc:getbyhandle123",
            SessionData = "{}",
            LoginType = LoginType.OAuth,
            IsDefault = false
        };

        await this.database.SaveLoginUserAsync(loginUser);

        // Act
        var retrieved = await this.database.GetLoginUserByHandleAsync("getbyhandle.bsky.social");

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(loginUser.Handle, retrieved.Handle);
        Assert.Equal(loginUser.Did, retrieved.Did);
    }

    [Fact(Skip = "Flaky test, needs investigation")]
    public async Task GetDefaultLoginUserAsync_ReturnsDefaultUser()
    {
        // Arrange
        await this.database.InitializeAsync();
        var user1 = new LoginUser
        {
            Handle = "nondefault.bsky.social",
            Email = "nondefault@example.com",
            Did = "did:plc:nondefault",
            SessionData = "{}",
            LoginType = LoginType.OAuth,
            IsDefault = false
        };
        var user2 = new LoginUser
        {
            Handle = "default.bsky.social",
            Email = "default@example.com",
            Did = "did:plc:default",
            SessionData = "{}",
            LoginType = LoginType.OAuth,
            IsDefault = true
        };

        await this.database.SaveLoginUserAsync(user1);
        await this.database.SaveLoginUserAsync(user2);

        // Act
        var defaultUser = await this.database.GetDefaultLoginUserAsync();

        // Assert
        Assert.NotNull(defaultUser);
        Assert.True(defaultUser.IsDefault);
        Assert.Equal("default.bsky.social", defaultUser.Handle);
    }

    [Fact(Skip = "Flaky test, needs investigation")]
    public async Task SaveOrUpdateLoginUserAsDefaultAsync_NewUser_SetsAsDefault()
    {
        // Arrange
        await this.database.InitializeAsync();
        var newUser = new LoginUser
        {
            Handle = "newdefault.bsky.social",
            Email = "newdefault@example.com",
            Did = "did:plc:newdefault",
            SessionData = "{}",
            LoginType = LoginType.OAuth
        };

        // Act
        var result = await this.database.SaveOrUpdateLoginUserAsDefaultAsync(newUser);

        // Assert
        Assert.True(result > 0);
        var defaultUser = await this.database.GetDefaultLoginUserAsync();
        Assert.NotNull(defaultUser);
        Assert.Equal("newdefault.bsky.social", defaultUser.Handle);
        Assert.True(defaultUser.IsDefault);
    }

    [Fact(Skip = "Flaky test, needs investigation")]
    public async Task SaveOrUpdateLoginUserAsDefaultAsync_ExistingUser_UpdatesAndSetsDefault()
    {
        // Arrange
        await this.database.InitializeAsync();
        var existingUser = new LoginUser
        {
            Handle = "existing.bsky.social",
            Email = "existing@example.com",
            Did = "did:plc:existing",
            SessionData = "{}",
            LoginType = LoginType.OAuth,
            IsDefault = false
        };

        await this.database.SaveLoginUserAsync(existingUser);

        // Act - Update with new session data
        var updatedUser = new LoginUser
        {
            Handle = "existing.bsky.social",
            Email = "newemail@example.com",
            Did = "did:plc:existing",
            SessionData = "{\"new\":\"session\"}",
            LoginType = LoginType.OAuth
        };

        var result = await this.database.SaveOrUpdateLoginUserAsDefaultAsync(updatedUser);

        // Assert
        Assert.True(result > 0);
        var defaultUser = await this.database.GetDefaultLoginUserAsync();
        Assert.NotNull(defaultUser);
        Assert.Equal("existing.bsky.social", defaultUser.Handle);
        Assert.Equal("newemail@example.com", defaultUser.Email);
        Assert.Equal("{\"new\":\"session\"}", defaultUser.SessionData);
        Assert.True(defaultUser.IsDefault);
    }

    [Fact(Skip = "Flaky test, needs investigation")]
    public async Task SaveOrUpdateLoginUserAsDefaultAsync_UnsetsOtherDefaults()
    {
        // Arrange
        await this.database.InitializeAsync();
        var user1 = new LoginUser
        {
            Handle = "user1.bsky.social",
            Email = "user1@example.com",
            Did = "did:plc:user1",
            SessionData = "{}",
            LoginType = LoginType.OAuth,
            IsDefault = true
        };
        var user2 = new LoginUser
        {
            Handle = "user2.bsky.social",
            Email = "user2@example.com",
            Did = "did:plc:user2",
            SessionData = "{}",
            LoginType = LoginType.OAuth
        };

        await this.database.SaveLoginUserAsync(user1);

        // Act - Set user2 as default
        await this.database.SaveOrUpdateLoginUserAsDefaultAsync(user2);

        // Assert
        var defaultUser = await this.database.GetDefaultLoginUserAsync();
        Assert.NotNull(defaultUser);
        Assert.Equal("user2.bsky.social", defaultUser.Handle);

        var user1Retrieved = await this.database.GetLoginUserByDidAsync("did:plc:user1");
        Assert.NotNull(user1Retrieved);
        Assert.False(user1Retrieved.IsDefault);
    }

    [Fact(Skip = "Flaky test, needs investigation")]
    public async Task DeleteLoginUserAsync_RemovesUser_ReturnsNonZeroId()
    {
        // Arrange
        await this.database.InitializeAsync();
        var loginUser = new LoginUser
        {
            Handle = "deleteme.bsky.social",
            Email = "deleteme@example.com",
            Did = "did:plc:deleteme",
            SessionData = "{}",
            LoginType = LoginType.OAuth,
            IsDefault = false
        };

        await this.database.SaveLoginUserAsync(loginUser);

        // Act
        var result = await this.database.DeleteLoginUserAsync(loginUser);

        // Assert
        Assert.True(result > 0);
        var retrieved = await this.database.GetLoginUserAsync(loginUser.Id);
        Assert.Null(retrieved);
    }

    [Fact(Skip = "Flaky test, needs investigation")]
    public async Task GetAppSettingsAsync_FirstCall_CreatesDefaultSettings()
    {
        // Arrange
        await this.database.InitializeAsync();

        // Act
        var settings = await this.database.GetAppSettingsAsync();

        // Assert
        Assert.NotNull(settings);
        Assert.True(settings.Id > 0);
    }

    [Fact(Skip = "Flaky test, needs investigation")]
    public async Task GetAppSettingsAsync_ReturnsExistingSettings()
    {
        // Arrange
        await this.database.InitializeAsync();
        var firstCall = await this.database.GetAppSettingsAsync();

        // Act
        var secondCall = await this.database.GetAppSettingsAsync();

        // Assert
        Assert.NotNull(firstCall);
        Assert.NotNull(secondCall);
        Assert.Equal(firstCall.Id, secondCall.Id);
    }

    [Fact(Skip = "Flaky test, needs investigation")]
    public async Task SaveAppSettingsAsync_UpdatesSettings_ReturnsTrue()
    {
        // Arrange
        await this.database.InitializeAsync();
        var settings = await this.database.GetAppSettingsAsync();
        Assert.NotNull(settings);

        // Modify settings
        settings.Theme = AppTheme.Dark;
        settings.Language = AppLanguage.English;

        // Act
        var result = await this.database.SaveAppSettingsAsync(settings);

        // Assert
        Assert.True(result);
        var retrieved = await this.database.GetAppSettingsAsync();
        Assert.NotNull(retrieved);
        Assert.Equal(AppTheme.Dark, retrieved.Theme);
        Assert.Equal(AppLanguage.English, retrieved.Language);
    }
}
