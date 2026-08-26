namespace OrganisationRegistry.ElasticSearch.Projections.Infrastructure;

using System;
using System.Collections.Generic;
using System.Linq;

public static class DocumentCacheExtensions
{
    /// <summary>
    /// Documents without a key or a name would corrupt the index, so they are never written.
    /// The offending documents are named in the exception: the cache spans an entire batch, so the
    /// envelope a flush happens on is usually not the envelope that produced the broken document.
    /// A document's changeId points at the envelope that last touched it.
    /// </summary>
    public static void ThrowOnDocumentsWithoutKeyOrName<T>(
        this Dictionary<Guid, T> documentCache,
        string projectionName)
        where T : class, IDocument
    {
        var invalidDocuments = documentCache
            .Where(x => x.Key == Guid.Empty || x.Value is null || string.IsNullOrEmpty(x.Value.Name))
            .Select(x => Describe(x.Key, x.Value))
            .ToList();

        if (!invalidDocuments.Any())
            return;

        throw new Exception(
            $"[{projectionName}] Found document without key or name. " +
            $"{invalidDocuments.Count} invalid {typeof(T).Name}(s): {string.Join(", ", invalidDocuments)}.");
    }

    private static string Describe<T>(Guid key, T? document)
        where T : class, IDocument
        => document switch
        {
            null => $"{key} (no document found in ElasticSearch)",
            _ when key == Guid.Empty => $"empty key (changeId {document.ChangeId})",
            _ => $"{key} (empty name, changeId {document.ChangeId})",
        };
}
