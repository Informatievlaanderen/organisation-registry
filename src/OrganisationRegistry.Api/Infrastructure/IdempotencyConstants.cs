namespace OrganisationRegistry.Api.Infrastructure;

/// <summary>
/// Centralizes the configuration values used by the <c>[Idempotent]</c> attribute
/// (from the IdempotentAPI package), so every endpoint that supports idempotent
/// retries agrees on the same header name and cache lifetime, and typos in the
/// (attribute-only, compile-time constant) header name are caught at compile time.
/// </summary>
public static class IdempotencyConstants
{
    /// <summary>
    /// The HTTP header integrators must send to make a Create request idempotent.
    /// Matches the IETF draft standard (draft-ietf-httpapi-idempotency-key-header)
    /// rather than the IdempotentAPI package's own default ("IdempotencyKey").
    /// </summary>
    public const string HeaderName = "Idempotency-Key";

    /// <summary>
    /// How long a cached response for a given idempotency key remains valid.
    /// Bounds the memory used by the (in-memory, single-instance) idempotency
    /// cache, while still covering realistic client retry windows (timeouts,
    /// network blips, retried batch jobs).
    /// </summary>
    public const int ExpireHours = 24;
}
