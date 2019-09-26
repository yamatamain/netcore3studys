using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Console.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            MemoryCache<string, string> _memoryCache = new MemoryCache<string, string>();
            CodeTimer.Initialize();
            CodeTimer.Time("object lock", 1000, () =>
            {
                var lazyStr = new Lazy<string>(() => Thread.CurrentThread.ManagedThreadId.ToString());
                _memoryCache.GetValueByObjectLocker("123", lazyStr);
            });

            CodeTimer.Time("LockSlim", 1000, () =>
            {
                var lazyStr = new Lazy<string>(() => Thread.CurrentThread.ManagedThreadId.ToString());
                _memoryCache.GetValueByLockSlim("456", lazyStr);
            });
            System.Console.WriteLine("123");
            System.Console.ReadLine();
        }
    }

    public class MemoryCache<TKey, TValue>
    {
        private ConcurrentDictionary<TKey, TValue> _dicCache = new ConcurrentDictionary<TKey, TValue>();

        private Dictionary<TKey, Lazy<TValue>> _dicLazyValue = new Dictionary<TKey, Lazy<TValue>>();

        private ReaderWriterLockSlim _cacheLock = new ReaderWriterLockSlim();

        private object _locker = new object();

        public TValue GetValueByObjectLocker(TKey key, Lazy<TValue> value)
        {
            if (!_dicLazyValue.ContainsKey(key))
            {
                lock (_locker)
                {
                    if (!_dicLazyValue.ContainsKey(key))
                    {
                        _dicLazyValue.Add(key, value);
                    }
                }
            }
            if (_dicCache == null)
            {
                lock (_locker)
                {
                    if (_dicCache == null)
                    {
                        _dicCache = new ConcurrentDictionary<TKey, TValue>();
                    }
                }
            }
            return _dicCache.GetOrAdd(key, _dicLazyValue[key].Value);
        }


        public TValue GetValueByLockSlim(TKey key, Lazy<TValue> value)
        {
            if (!_dicLazyValue.ContainsKey(key))
            {
                try
                {
                    _cacheLock.EnterWriteLock();
                    if (!_dicLazyValue.ContainsKey(key))
                    {
                        _dicLazyValue.Add(key, value);
                    }
                }
                finally
                {
                    _cacheLock.ExitWriteLock();
                }
            }
            if (_dicCache == null)
            {
                try
                {
                    _cacheLock.EnterUpgradeableReadLock();
                    if (_dicCache == null)
                    {
                        _dicCache = new ConcurrentDictionary<TKey, TValue>();
                    }
                }
                finally
                {
                    _cacheLock.ExitUpgradeableReadLock();
                }
            }
            return _dicCache.GetOrAdd(key, _dicLazyValue[key].Value);
        }
    }
}
