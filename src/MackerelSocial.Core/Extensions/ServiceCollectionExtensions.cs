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
        services.AddTransient<ViewModels.LoginViewModel>();
        services.AddTransient<ViewModels.LaunchViewModel>();
        services.AddTransient<ViewModels.ThreadViewPostViewModel>();
        services.AddTransient<ViewModels.AuthorViewModel>();
        services.AddTransient<ViewModels.PopularFeedGeneratorViewModel>();
        services.AddSingleton<MackerelSocial.ViewModels.Factories.IAuthorViewModelFactory, MackerelSocial.ViewModels.Factories.AuthorViewModelFactory>();
        services.AddSingleton<MackerelSocial.ViewModels.Factories.IPopularFeedGeneratorViewModelFactory, MackerelSocial.ViewModels.Factories.PopularFeedGeneratorViewModelFactory>();
        services.AddSingleton<MackerelSocial.ViewModels.Factories.IThreadViewPostViewModelFactory, MackerelSocial.ViewModels.Factories.ThreadViewPostViewModelFactory>();
        return services;
    }
}
