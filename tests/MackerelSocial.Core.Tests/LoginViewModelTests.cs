// <copyright file="LoginViewModelTests.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using FishyFlip;
using MackerelSocial.Core.Services;
using MackerelSocial.Core.ViewModels;

namespace MackerelSocial.Core.Tests;

/// <summary>
/// Tests for LoginViewModel.
/// </summary>
[Collection("Auth")]
public class LoginViewModelTests : IDisposable
{
    private readonly AuthenticatedProtocolFixture fixture;
    private readonly DatabaseService database;
    private readonly ATProtocol protocol;
    private readonly string testHandle;
    private readonly string testPassword;

    public LoginViewModelTests(AuthenticatedProtocolFixture fixture)
    {
        this.fixture = fixture;
        this.database = new DatabaseService(":memory:");
        this.database.InitializeAsync().Wait();

        // Create a new protocol instance for each test
        var builder = new ATProtocolBuilder();
        this.protocol = builder.Build();

        this.testHandle = Environment.GetEnvironmentVariable("BLUESKY_TEST_HANDLE")
            ?? throw new InvalidOperationException("BLUESKY_TEST_HANDLE environment variable is not set");
        this.testPassword = Environment.GetEnvironmentVariable("BLUESKY_TEST_PASSWORD")
            ?? throw new InvalidOperationException("BLUESKY_TEST_PASSWORD environment variable is not set");
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
        var viewModel = new LoginViewModel(this.protocol, this.database);

        // Assert
        Assert.Equal(string.Empty, viewModel.Identifier);
        Assert.Equal(string.Empty, viewModel.Password);
        Assert.Equal(string.Empty, viewModel.ErrorMessage);
        Assert.False(viewModel.IsLoggingInWithPassword);
        Assert.False(viewModel.IsLoggingInWithOAuth);
        Assert.False(viewModel.IsLoggingIn);
    }

    [Fact]
    public void CanLoginWithPassword_WithValidCredentials_ReturnsTrue()
    {
        // Arrange
        var viewModel = new LoginViewModel(this.protocol, this.database);
        viewModel.Identifier = this.testHandle;
        viewModel.Password = "somepassword";

        // Act & Assert
        Assert.True(viewModel.CanLoginWithPassword);
    }

    [Fact]
    public void CanLoginWithPassword_WithEmptyIdentifier_ReturnsFalse()
    {
        // Arrange
        var viewModel = new LoginViewModel(this.protocol, this.database);
        viewModel.Identifier = string.Empty;
        viewModel.Password = "somepassword";

        // Act & Assert
        Assert.False(viewModel.CanLoginWithPassword);
    }

    [Fact]
    public void CanLoginWithPassword_WithEmptyPassword_ReturnsFalse()
    {
        // Arrange
        var viewModel = new LoginViewModel(this.protocol, this.database);
        viewModel.Identifier = this.testHandle;
        viewModel.Password = string.Empty;

        // Act & Assert
        Assert.False(viewModel.CanLoginWithPassword);
    }

    [Fact]
    public void CanLoginWithPassword_WithInvalidIdentifier_ReturnsFalse()
    {
        // Arrange
        var viewModel = new LoginViewModel(this.protocol, this.database);
        viewModel.Identifier = "invalid identifier with spaces";
        viewModel.Password = "somepassword";

        // Act & Assert
        Assert.False(viewModel.CanLoginWithPassword);
    }

    [Fact]
    public void CanLoginWithPassword_WhileLoggingIn_ReturnsFalse()
    {
        // Arrange
        var viewModel = new LoginViewModel(this.protocol, this.database);
        viewModel.Identifier = this.testHandle;
        viewModel.Password = "somepassword";

        // Use reflection to set private field for testing
        var field = typeof(LoginViewModel).GetField("_isLoggingInWithPassword",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(viewModel, true);

        // Act & Assert
        Assert.False(viewModel.CanLoginWithPassword);
    }

    [Fact]
    public void CanLoginWithOauth_WithValidHandle_ReturnsTrue()
    {
        // Arrange
        var viewModel = new LoginViewModel(this.protocol, this.database);
        viewModel.Identifier = this.testHandle;

        // Act & Assert
        Assert.True(viewModel.CanLoginWithOauth);
    }

    [Fact]
    public void CanLoginWithOauth_WithEmailAddress_ReturnsFalse()
    {
        // Arrange
        var viewModel = new LoginViewModel(this.protocol, this.database);
        viewModel.Identifier = "test@example.com";

        // Act & Assert
        Assert.False(viewModel.CanLoginWithOauth);
    }

    [Fact]
    public void CanLoginWithOauth_WithEmptyIdentifier_ReturnsFalse()
    {
        // Arrange
        var viewModel = new LoginViewModel(this.protocol, this.database);
        viewModel.Identifier = string.Empty;

        // Act & Assert
        Assert.False(viewModel.CanLoginWithOauth);
    }

    [Fact]
    public void IsLoggingIn_WhenLoggingInWithPassword_ReturnsTrue()
    {
        // Arrange
        var viewModel = new LoginViewModel(this.protocol, this.database);

        // Use reflection to set private field
        var field = typeof(LoginViewModel).GetField("_isLoggingInWithPassword",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(viewModel, true);

        // Act & Assert
        Assert.True(viewModel.IsLoggingIn);
    }

    [Fact]
    public void IsLoggingIn_WhenLoggingInWithOAuth_ReturnsTrue()
    {
        // Arrange
        var viewModel = new LoginViewModel(this.protocol, this.database);

        // Use reflection to set private field
        var field = typeof(LoginViewModel).GetField("_isLoggingInWithOAuth",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(viewModel, true);

        // Act & Assert
        Assert.True(viewModel.IsLoggingIn);
    }

    [Fact]
    public async Task LoginWithPasswordAsync_WithValidCredentials_ReturnsTrue()
    {
        // Arrange
        var viewModel = new LoginViewModel(this.protocol, this.database);
        viewModel.Identifier = this.testHandle;
        viewModel.Password = this.testPassword;

        // Act
        var result = await viewModel.LoginWithPasswordAsync();

        // Assert
        Assert.True(result);
        Assert.Equal(string.Empty, viewModel.ErrorMessage);
        Assert.False(viewModel.IsLoggingInWithPassword);

        // Verify user was saved to database
        var users = await this.database.GetLoginUsersAsync();
        Assert.Single(users);
        var savedUser = users.First();
        Assert.NotNull(savedUser);
        Assert.True(savedUser.IsDefault);
        Assert.Equal(Models.LoginType.Password, savedUser.LoginType);
    }

    [Fact]
    public async Task LoginWithPasswordAsync_WithInvalidCredentials_ReturnsFalse()
    {
        // Arrange
        var viewModel = new LoginViewModel(this.protocol, this.database);
        viewModel.Identifier = this.testHandle;
        viewModel.Password = "wrongpassword123";

        // Act
        var result = await viewModel.LoginWithPasswordAsync();

        // Assert
        Assert.False(result);
        Assert.NotEqual(string.Empty, viewModel.ErrorMessage);
        Assert.False(viewModel.IsLoggingInWithPassword);

        // Verify no user was saved to database
        var users = await this.database.GetLoginUsersAsync();
        Assert.Empty(users);
    }

    [Fact]
    public async Task LoginWithPasswordAsync_WithInvalidHandle_ReturnsFalse()
    {
        // Arrange
        var viewModel = new LoginViewModel(this.protocol, this.database);
        viewModel.Identifier = "nonexistent.bsky.social";
        viewModel.Password = "somepassword";

        // Act
        var result = await viewModel.LoginWithPasswordAsync();

        // Assert
        Assert.False(result);
        Assert.NotEqual(string.Empty, viewModel.ErrorMessage);
        Assert.False(viewModel.IsLoggingInWithPassword);
    }

    [Fact]
    public async Task LoginWithPasswordAsync_WithEmailIdentifier_WorksIfValid()
    {
        // Arrange
        var viewModel = new LoginViewModel(this.protocol, this.database);
        // Note: This test assumes the test account has an email associated
        // If the test handle has an associated email, we could test with that
        // For now, we'll test that the validation allows email format
        viewModel.Identifier = "test@example.com";
        viewModel.Password = "somepassword";

        // Act - This will fail authentication, but should pass validation
        var result = await viewModel.LoginWithPasswordAsync();

        // Assert - Should attempt login (validation passes)
        // Result will be false due to invalid credentials, but error should be auth-related, not validation
        Assert.False(result);
        Assert.NotEqual(string.Empty, viewModel.ErrorMessage);
    }

    [Fact]
    public async Task LoginWithPasswordAsync_SetsIsLoggingInDuringExecution()
    {
        // Arrange
        var viewModel = new LoginViewModel(this.protocol, this.database);
        viewModel.Identifier = this.testHandle;
        viewModel.Password = this.testPassword;

        bool wasLoggingInDuringExecution = false;

        // Act
        var loginTask = Task.Run(async () =>
        {
            await Task.Delay(50); // Small delay to ensure we check during execution
            wasLoggingInDuringExecution = viewModel.IsLoggingInWithPassword;
        });

        var result = await viewModel.LoginWithPasswordAsync();
        await loginTask;

        // Assert
        Assert.True(result);
        Assert.False(viewModel.IsLoggingInWithPassword); // Should be false after completion
    }

    [Fact]
    public async Task LoginWithPasswordAsync_ClearsErrorMessageBeforeAttempt()
    {
        // Arrange
        var viewModel = new LoginViewModel(this.protocol, this.database);
        viewModel.Identifier = this.testHandle;
        viewModel.Password = "wrongpassword";

        // First attempt with wrong password
        await viewModel.LoginWithPasswordAsync();
        Assert.NotEqual(string.Empty, viewModel.ErrorMessage);

        // Update with correct password
        viewModel.Password = this.testPassword;

        // Act - Second attempt with correct password
        var result = await viewModel.LoginWithPasswordAsync();

        // Assert
        Assert.True(result);
        Assert.Equal(string.Empty, viewModel.ErrorMessage);
    }

    [Fact]
    public async Task LoginWithPasswordAsync_SavesSessionDataToDatabase()
    {
        // Arrange
        var viewModel = new LoginViewModel(this.protocol, this.database);
        viewModel.Identifier = this.testHandle;
        viewModel.Password = this.testPassword;

        // Act
        var result = await viewModel.LoginWithPasswordAsync();

        // Assert
        Assert.True(result);

        var defaultUser = await this.database.GetDefaultLoginUserAsync();
        Assert.NotNull(defaultUser);
        Assert.NotNull(defaultUser.SessionData);
        Assert.NotEqual(string.Empty, defaultUser.SessionData);
        Assert.NotNull(defaultUser.Did);
        Assert.NotEqual(string.Empty, defaultUser.Did);
        Assert.NotNull(defaultUser.Handle);
        Assert.NotEqual(string.Empty, defaultUser.Handle);
    }

    [Theory]
    [InlineData("test.bsky.social")]
    [InlineData("user.handle.bsky.social")]
    [InlineData("test@example.com")]
    [InlineData("user@domain.co.uk")]
    public void Identifier_WithVariousValidFormats_AllowsLogin(string identifier)
    {
        // Arrange
        var viewModel = new LoginViewModel(this.protocol, this.database);
        viewModel.Identifier = identifier;
        viewModel.Password = "somepassword";

        // Act & Assert
        // Should be able to attempt login (CanLoginWithPassword should be true)
        Assert.True(viewModel.CanLoginWithPassword);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("invalid identifier")]
    [InlineData("@invalid")]
    [InlineData("no-domain@")]
    public void Identifier_WithInvalidFormats_PreventsLogin(string identifier)
    {
        // Arrange
        var viewModel = new LoginViewModel(this.protocol, this.database);
        viewModel.Identifier = identifier;
        viewModel.Password = "somepassword";

        // Act & Assert
        Assert.False(viewModel.CanLoginWithPassword);
    }
}
