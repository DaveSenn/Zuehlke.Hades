using System.Collections.Generic;

namespace Zuehlke.Hades.Matcher
{
    /// <summary>
    /// A simple implementation of an LRU-Cache (least recently used)
    /// </summary>
    /// <typeparam name="K">Key type</typeparam>
    /// <typeparam name="V">Value type</typeparam>
    public class LRUCache<K, V>
    {
        private readonly object _locker = new object();
        private readonly int _capacity;
        private readonly Dictionary<K, LinkedListNode<LRUCacheItem<K, V>>> _cacheMap = new Dictionary<K, LinkedListNode<LRUCacheItem<K, V>>>();
        private readonly LinkedList<LRUCacheItem<K, V>> _lruList = new LinkedList<LRUCacheItem<K, V>>();

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
        /// <param name="key">Key of type <see cref="K"/></param>
        /// <returns>Value of type <see cref="V"/> for the given key</returns>
        public V Get(K key)
        {
            lock (_locker)
            {
                LinkedListNode<LRUCacheItem<K, V>> node;
                if (_cacheMap.TryGetValue(key, out node))
                {
                    V value = node.Value.value;
                    _lruList.Remove(node);
                    _lruList.AddLast(node);
                    return value;
                }
                return default(V);
            }
        }
        
        /// <summary>
        /// Caches a new value for the given key
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="val">Value that should be cached</param>
        public void Add(K key, V val)
        {
            lock (_locker)
            {
                if (_cacheMap.Count >= _capacity)
                {
                    RemoveFirst();
                }

                LRUCacheItem<K, V> cacheItem = new LRUCacheItem<K, V>(key, val);
                LinkedListNode<LRUCacheItem<K, V>> node = new LinkedListNode<LRUCacheItem<K, V>>(cacheItem);
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
            LinkedListNode<LRUCacheItem<K, V>> node = _lruList.First;
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
