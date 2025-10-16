using FishyFlip.Lexicon.App.Bsky.Unspecced;

namespace MackerelSocial.Core.ViewModels.Factories;

public interface IPopularFeedGeneratorViewModelFactory
{
    PopularFeedGeneratorViewModel Create(Models.LoginUser? currentUser = default);
    PopularFeedGeneratorViewModel Create(string query, Models.LoginUser? currentUser = default);
}