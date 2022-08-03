namespace OrganisationRegistry.Api.Security;

using System.Collections.Generic;
using System.ComponentModel;

public class BiDirectionalDictionary<TForwardKey, TReverseKey>
    where TForwardKey : notnull
    where TReverseKey : notnull
{
    private readonly Dictionary<TForwardKey, TReverseKey> _forward = new();
    private readonly Dictionary<TReverseKey, TForwardKey> _reverse = new();

    public BiDirectionalDictionary()
    {
        Forward = new Indexer<TForwardKey, TReverseKey>(_forward);
        Reverse = new Indexer<TReverseKey, TForwardKey>(_reverse);
    }

    public class Indexer<TKey, TValue>
        where TKey : notnull
        where TValue : notnull
    {
        private readonly Dictionary<TKey, TValue> _dictionary;

        public Indexer(Dictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
        }

        public TValue this[TKey index]
        {
            get => _dictionary[index];
            set => _dictionary[index] = value;
        }

        public bool ContainsKey(TKey key)
            => _dictionary.ContainsKey(key);
    }

    public void Add(TForwardKey forwardKey, TReverseKey reverseKey)
    {
        if (_forward.ContainsKey(forwardKey))
            throw new InvalidEnumArgumentException($"Key {forwardKey} already exists in Forward dictionary.");
        if (_reverse.ContainsKey(reverseKey))
            throw new InvalidEnumArgumentException($"Key {reverseKey} already exists in Reverse dictionary.");

        _forward.Add(forwardKey, reverseKey);
        _reverse.Add(reverseKey, forwardKey);
    }

    private Indexer<TForwardKey, TReverseKey> Forward { get; }
    private Indexer<TReverseKey, TForwardKey> Reverse { get; }

    public TReverseKey this[TForwardKey index]
        => Forward[index];

    public TForwardKey this[TReverseKey index]
        => Reverse[index];

    public bool ContainsKey(TForwardKey key)
        => Forward.ContainsKey(key);

    public bool ContainsKey(TReverseKey key)
        => Reverse.ContainsKey(key);
}
