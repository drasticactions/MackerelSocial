// <copyright file="TestConstants.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace MackerelSocial.Core.Tests;

/// <summary>
/// Test constants for live Bluesky data.
/// </summary>
public static class TestConstants
{
    /// <summary>
    /// Test Bluesky handle.
    /// </summary>
    public const string TestHandle = "drasticactions.xn--q9jyb4c";

    /// <summary>
    /// Test Bluesky DID.
    /// </summary>
    public const string TestDid = "did:plc:okblbaji7rz243bluudjlgxt";

    /// <summary>
    /// Test post URI.
    /// </summary>
    public const string TestPostUri = "at://drasticactions.dev/app.bsky.feed.post/3m367otk7mc2e";

    /// <summary>
    /// Test feed generator URI.
    /// </summary>
    public const string TestFeedUri = "at://did:plc:cgl62jlhroosxyjkaffidnon/app.bsky.feed.generator/aaae4nczs635m";

    /// <summary>
    /// Test search query.
    /// </summary>
    public const string TestSearchQuery = "art";

    /// <summary>
    /// Default timeout for API calls in milliseconds.
    /// </summary>
    public const int DefaultTimeoutMs = 30000;
}
