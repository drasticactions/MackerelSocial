// <copyright file="OnLoginUserEventArgs.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using MackerelSocial.Core.Models;

namespace MackerelSocial.Core.Events;

public class OnLoginUserEventArgs : EventArgs
{
    public OnLoginUserEventArgs(LoginUser loginUser, DatabaseEvent databaseEvent)
    {
        this.LoginUser = loginUser;
        this.Event = databaseEvent;
    }

    public LoginUser LoginUser { get; }

    public DatabaseEvent Event { get; }
}