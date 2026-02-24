using Kanban.Domain.Abstractions.Services;

namespace Kanban.Infrastructure.Services;

/// <summary>
/// Default implementation of <see cref="IClock"/> using system time.
/// </summary>
public sealed class SystemClock : IClock
{
    /// <inheritdoc />
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}