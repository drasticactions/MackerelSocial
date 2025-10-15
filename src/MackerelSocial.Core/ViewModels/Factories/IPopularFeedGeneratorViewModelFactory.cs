using FishyFlip.Lexicon.App.Bsky.Unspecced;

namespace MackerelSocial.Core.ViewModels.Factories;

public interface IPopularFeedGeneratorViewModelFactory
{
    PopularFeedGeneratorViewModel Create();
    PopularFeedGeneratorViewModel Create(string query);
}