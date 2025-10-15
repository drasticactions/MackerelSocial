# MackerelSocial

MackerelSocial is a Bluesky client application, built on .NET.

# Design

MackerelSocial is designed to be cross-platform, with a `MackerelSocial.Core` library that contains the base logic for dealing with Bluesky/ATProtocol, a database implementation for storing data, and interfaces that are then consumed by the UI Platform applications. These can be platform native apps (like .NET iOS, .NET Android, WinUI, etc.) or cross-platform frameworks like Avalonia and MAUI.

The first goal is to implement the functionality needed in `MackerelSocial.Core` first, then add tests for them in `MackerelSocial.Core.Tests`. Once ready, we can then start UI Projects with new platform projects and test harnesses to go along with them.

# Build

## MackerelSocial.Core

`dotnet build src/MackerelSocial.Core`

# Test

## MackerelSocial.Core.Tests

`dotnet test tests/MackerelSocial.Core.Tests`

# Libraries

- [FishyFlip](https://github.com/drasticactions/FishyFlip) - For ATProtocol/Bluesky functions. This is submoduled in [external/FishyFlip](external/FishyFlip/).
- sqlite-net-pcl - For Database Functions
- Microsoft.Extensions.DependencyInjection