using System.Collections.Generic;

namespace Zuehlke.Hades.Matcher
{
    /// <summary>
    /// A simple implementation of an LRU-Cache (least recently used)
    /// </summary>
    /// <typeparam name="TK">Key type</typeparam>
    /// <typeparam name="TV">Value type</typeparam>
    public class LRUCache<TK, TV>
    {
        private readonly object _locker = new object();
        private readonly int _capacity;
        private readonly Dictionary<TK, LinkedListNode<LRUCacheItem<TK, TV>>> _cacheMap = new Dictionary<TK, LinkedListNode<LRUCacheItem<TK, TV>>>();
        private readonly LinkedList<LRUCacheItem<TK, TV>> _lruList = new LinkedList<LRUCacheItem<TK, TV>>();

        /// <summary>
        /// Initializes a new instance of an <see cref="LRUCache{K, V}"/> with the given capacity.
        /// </summary>
        /// <param name="capacity">The maximum amout of items the cache should keep</param>
        public LRUCache(int capacity)
        {
            _capacity = capacity;
        }
        
        /// <summary>
        /// Get the value for a given key from the cache
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Value for the given key</returns>
        public TV Get(TK key)
        {
            lock (_locker)
            {
                LinkedListNode<LRUCacheItem<TK, TV>> node;
                if (_cacheMap.TryGetValue(key, out node))
                {
                    TV value = node.Value.value;
                    _lruList.Remove(node);
                    _lruList.AddLast(node);
                    return value;
                }
                return default(TV);
            }
        }
        
        /// <summary>
        /// Caches a new value for the given key
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="val">Value that should be cached</param>
        public void Add(TK key, TV val)
        {
            lock (_locker)
            {
                if (_cacheMap.Count >= _capacity)
                {
                    RemoveFirst();
                }

                LRUCacheItem<TK, TV> cacheItem = new LRUCacheItem<TK, TV>(key, val);
                LinkedListNode<LRUCacheItem<TK, TV>> node = new LinkedListNode<LRUCacheItem<TK, TV>>(cacheItem);
                _lruList.AddLast(node);
                _cacheMap.Add(key, node);
            }
        }

        /// <summary>
        /// Removes the first item from the cache
        /// </summary>
        private void RemoveFirst()
        {
            // Remove from LRUPriority
            LinkedListNode<LRUCacheItem<TK, TV>> node = _lruList.First;
            _lruList.RemoveFirst();

            // Remove from cache
            _cacheMap.Remove(node.Value.key);
        }
    }

    /// <summary>
    /// A simple cache item
    /// </summary>
    /// <typeparam name="K">Key type</typeparam>
    /// <typeparam name="V">Value type</typeparam>
    class LRUCacheItem<K, V>
    {
        public LRUCacheItem(K k, V v)
        {
            key = k;
            value = v;
        }
        public K key;
        public V value;
    }
}
