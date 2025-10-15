using FishyFlip;
using FishyFlip.Models;

namespace MackerelSocial.Core.ViewModels.Factories;

public interface IAuthorViewModelFactory
{
    AuthorViewModel Create(ATIdentifier identifier, string filter = "");
}