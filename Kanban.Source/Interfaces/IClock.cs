namespace Kanban.Source.Interfaces;

/// <summary>
/// Time provider abstraction (improves testability).
/// </summary>
public interface IClock
{
    /// <summary>
    /// Gets the current UTC time.
    /// </summary>
    DateTimeOffset UtcNow { get; }
}