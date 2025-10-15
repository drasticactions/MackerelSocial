using FishyFlip;
using FishyFlip.Models;

namespace MackerelSocial.ViewModels.Factories;

public interface IAuthorViewModelFactory
{
    AuthorViewModel Create(ATIdentifier identifier, string filter = "");
}