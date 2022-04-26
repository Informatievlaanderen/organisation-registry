namespace OrganisationRegistry.Infrastructure.Authorization.Cache
{
    using System;
    using System.Threading.Tasks;

    public interface ICache<T>
    {
        Task<T> GetOrAdd(string key, Func<Task<T>> getItemIfNotInCache);
        void Set(string key, T item);
        void Expire(string key);
        void ExpireAll();
    }
}
