namespace OrganisationRegistry.Api.IntegrationTests;

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

/// <summary>
/// A composite disposable that manages multiple disposable resources.
/// Disposes all contained resources when the composite is disposed.
/// Useful for cleaning up multiple httpclients, database connections, etc.
/// </summary>
public sealed class CompositeDisposable(ILogger? logger) : IDisposable
{
    private readonly List<IDisposable> _disposables = new();
    private readonly ILogger? _logger = logger;
    private bool _disposed;

    public CompositeDisposable() : this(null)
    {
    }

    public CompositeDisposable Add(IDisposable disposable)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (disposable != null)
            _disposables.Add(disposable);

        return this;
    }

    public CompositeDisposable AddRange(params IDisposable[] disposables)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        foreach (var disposable in disposables)
        {
            if (disposable != null)
                _disposables.Add(disposable);
        }

        return this;
    }

    public int Count
        => _disposed ? 0 : _disposables.Count;

    public bool IsDisposed
        => _disposed;

    public void Dispose()
    {
        if (_disposed)
            return;

        // Dispose in reverse order (LIFO) of addition for proper dependency cleanup
        for (int i = _disposables.Count - 1; i >= 0; i--)
        {
            var disposable = _disposables[i];
            try
            {
                disposable?.Dispose();
                _logger?.LogDebug("Successfully disposed resource of type {ResourceType}", disposable?.GetType().Name);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error disposing resource of type {ResourceType}", disposable?.GetType().Name);
                // Continue disposing remaining resources even if one fails
            }
        }

        _disposables.Clear();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}

public static class CompositeDisposableExtensions
{
    public static CompositeDisposable CreateComposite(this IDisposable _, ILogger? logger)
    {
        return new CompositeDisposable(logger);
    }

    public static T DisposeWith<T>(this T obj, CompositeDisposable composite) where T : class, IDisposable
    {
        composite.Add(obj);
        return obj;
    }
}
