using FishyFlip.Lexicon.App.Bsky.Feed;
using FishyFlip.Models;

namespace MackerelSocial.Core.ViewModels.Factories;

public interface IThreadViewPostViewModelFactory
{
    ThreadViewPostViewModel Create(ThreadViewPost post, Models.LoginUser? currentUser = default);
    ThreadViewPostViewModel Create(ATUri uri, Models.LoginUser? currentUser = default);
}