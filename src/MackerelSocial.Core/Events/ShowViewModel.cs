// <copyright file="OnLoginUserEventArgs.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using MackerelSocial.Models;

namespace MackerelSocial.Events;

public enum ShowViewModel
{
    None,
    Login,
    Main
}

public class ShowViewModelEventArgs : EventArgs
{
    public ShowViewModelEventArgs(ShowViewModel viewModel)
    {
        this.ViewModel = viewModel;
    }

    public ShowViewModel ViewModel { get; }
}