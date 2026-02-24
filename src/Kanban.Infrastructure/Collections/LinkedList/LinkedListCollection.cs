using Kanban.Domain.Abstractions.Collections;

namespace Kanban.Infrastructure.Collections.LinkedList;

/// <summary>
/// Singly linked list implementation of <see cref="IMyCollection{T}"/>.
/// This class does not rely on System.Collections.Generic.
/// </summary>
/// <typeparam name="T">Element type.</typeparam>