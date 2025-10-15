namespace MackerelSocial.Models;

/// <summary>
/// Database Event.
/// </summary>
public enum DatabaseEvent
{
    /// <summary>
    /// Database Updated.
    /// </summary>
    Updated,

    /// <summary>
    /// Database Deleted.
    /// </summary>
    Deleted,

    /// <summary>
    /// Database Inserted.
    /// </summary>
    Inserted,
}