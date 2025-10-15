using FishyFlip.Lexicon.App.Bsky.Feed;
using FishyFlip.Models;

namespace MackerelSocial.ViewModels.Factories;

public interface IThreadViewPostViewModelFactory
{
    ThreadViewPostViewModel Create(ThreadViewPost post);
    ThreadViewPostViewModel Create(ATUri uri);
}