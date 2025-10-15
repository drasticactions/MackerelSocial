namespace MackerelSocial.Extensions;

using FishyFlip.Lexicon.App.Bsky.Labeler;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDefaultServices(this IServiceCollection services)
    {
        var atProtocolBuilder = new FishyFlip.ATProtocolBuilder();
        var atProtocol = atProtocolBuilder.Build();
        services.AddSingleton(atProtocol);
        services.AddTransient<Core.ViewModels.ThreadViewPostViewModel>();
        services.AddTransient<Core.ViewModels.AuthorViewModel>();
        services.AddTransient<Core.ViewModels.PopularFeedGeneratorViewModel>();
        services.AddSingleton<MackerelSocial.Core.ViewModels.Factories.IAuthorViewModelFactory, MackerelSocial.Core.ViewModels.Factories.AuthorViewModelFactory>();
        services.AddSingleton<MackerelSocial.Core.ViewModels.Factories.IPopularFeedGeneratorViewModelFactory, MackerelSocial.Core.ViewModels.Factories.PopularFeedGeneratorViewModelFactory>();
        services.AddSingleton<MackerelSocial.Core.ViewModels.Factories.IThreadViewPostViewModelFactory, MackerelSocial.Core.ViewModels.Factories.ThreadViewPostViewModelFactory>();
        return services;
    }
}
