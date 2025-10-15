using FishyFlip.Lexicon.App.Bsky.Unspecced;

namespace MackerelSocial.ViewModels.Factories;

public interface IPopularFeedGeneratorViewModelFactory
{
    PopularFeedGeneratorViewModel Create();
    PopularFeedGeneratorViewModel Create(string query);
}