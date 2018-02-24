/*
 * This file is part of the OpenNos Emulator Project. See AUTHORS file for Copyright information
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace OpenNos.Core
{
    public class ThreadSafeGenericList<T> : IDisposable
    {
        #region Members

        /// <summary>
        /// private collection to store _items.
        /// </summary>
        private readonly List<T> _items;

        /// <summary>
        /// Used to synchronize access to _items list.
        /// </summary>
        private readonly ReaderWriterLockSlim _lock;

        private bool _disposed;

        #endregion

        #region Instantiation

        /// <summary>
        /// Creates a new ThreadSafeGenericList object.
        /// </summary>
        public ThreadSafeGenericList()
        {
            _items = new List<T>();
            _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of elements contained in the List&lt;T&gt;.
        /// </summary>
        public int Count
        {
            get
            {
                if (!_disposed)
                {
                    _lock.EnterReadLock();
                    try
                    {
                        return _items.Count;
                    }
                    finally
                    {
                        _lock.ExitReadLock();
                    }
                }
                return 0;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds an object to the end of the List&lt;T&gt;.
        /// </summary>
        /// <param name="value"></param>
        public void Add(T value)
        {
            if (!_disposed)
            {
                _lock.EnterWriteLock();
                try
                {
                    _items.Add(value);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the List&lt;T&gt;.
        /// </summary>
        /// <param name="value"></param>
        public void AddRange(List<T> value)
        {
            if (!_disposed)
            {
                _lock.EnterWriteLock();
                try
                {
                    _items.AddRange(value);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }

        /// <summary>
        /// Determines whether all elements of a sequence satisfy a condition.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>True; if elements satisgy the condition</returns>
        public bool All(Func<T, bool> predicate)
        {
            if (!_disposed)
            {
                _lock.EnterReadLock();
                try
                {
                    return _items.All(predicate);
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether any element of a sequence satisfies a condition.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool Any(Func<T, bool> predicate)
        {
            if (!_disposed)
            {
                _lock.EnterReadLock();
                try
                {
                    return _items.Any(predicate);
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            return false;
        }

        /// <summary>
        /// Removes all elements from the List&lt;T&gt;.
        /// </summary>
        public void Clear()
        {
            if (!_disposed)
            {
                _lock.EnterWriteLock();
                try
                {
                    _items.Clear();
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }

        /// <summary>
        /// Copies the entire List&lt;T&gt; to a compatible one-dimensional array, starting at the
        /// beginning of the target array.
        /// </summary>
        /// <param name="grpmembers"></param>
        public void CopyTo(T[] grpmembers)
        {
            if (!_disposed)
            {
                _lock.EnterReadLock();
                try
                {
                    _items.CopyTo(grpmembers);
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// Returns a number that represents how many elements in the specified sequence satisfy a condition.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>integer number of found elements</returns>
        public int CountLinq(Func<T, bool> predicate)
        {
            if (!_disposed)
            {
                _lock.EnterReadLock();
                try
                {
                    return _items.Count(predicate);
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            return 0;
        }

        /// <summary>
        /// Disposes the current object.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Returns the element at given index
        /// </summary>
        /// <param name="v"></param>
        /// <returns>T object</returns>
        public T ElementAt(int v)
        {
            if (!_disposed)
            {
                _lock.EnterReadLock();
                try
                {
                    return _items[v];
                }
                catch
                {
                    Logger.Error("Tell Master that i dropped and IOR exception somewhere, also stop crying about it.");
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            return default;
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate,
        /// and returns the first occurrence within the entire List&lt;T&gt;.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>T object</returns>
        public T Find(Predicate<T> predicate)
        {
            if (!_disposed)
            {
                _lock.EnterReadLock();
                try
                {
                    return _items.Find(predicate);
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            return default;
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate,
        /// and returns the first occurrence within the entire List&lt;T&gt;.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>T object</returns>
        public T FirstOrDefault()
        {
            if (!_disposed)
            {
                _lock.EnterReadLock();
                try
                {
                    return _items.FirstOrDefault();
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            return default;
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate,
        /// and returns the first occurrence within the entire List&lt;T&gt;.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>T object</returns>
        public T FirstOrDefault(Func<T, bool> predicate)
        {
            if (!_disposed)
            {
                _lock.EnterReadLock();
                try
                {
#pragma warning disable RCS1119 // Call 'Find' instead of 'FirstOrDefault'.
                    return _items.FirstOrDefault(predicate);
#pragma warning restore RCS1119 // Call 'Find' instead of 'FirstOrDefault'.
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            return default;
        }

        /// <summary>
        /// Performs the specified action on each element of the List&lt;T&gt;.
        /// </summary>
        /// <param name="action"></param>
        public void ForEach(Action<T> action)
        {
            if (!_disposed)
            {
                _lock.EnterReadLock();
                try
                {
                    _items.ForEach(action);
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// returns a list of all objects in current thread safe generic list
        /// </summary>
        /// <returns>List&lt;T&gt;</returns>
        public List<T> GetAllItems()
        {
            if (!_disposed)
            {
                _lock.EnterReadLock();
                try
                {
                    return new List<T>(_items);
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            return new List<T>();
        }

        /// <summary>
        /// Returns the last element of a sequence that satisfies a condition or a default value if
        /// no such element is found.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>TV object</returns>
        public T LastOrDefault(Func<T, bool> predicate)
        {
            if (!_disposed)
            {
                _lock.EnterReadLock();
                try
                {
                    return _items.LastOrDefault(predicate);
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            return default;
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the List&lt;T&gt;.
        /// </summary>
        /// <param name="match"></param>
        public void Remove(T match)
        {
            if (!_disposed)
            {
                _lock.EnterWriteLock();
                try
                {
                    _items.Remove(match);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }

        /// <summary>
        /// Removes all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match"></param>
        public void RemoveAll(Predicate<T> match)
        {
            if (!_disposed)
            {
                _lock.EnterWriteLock();
                try
                {
                    _items.RemoveAll(match);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }

        /// <summary>
        /// Returns the only element of a sequence that satisfies a specified condition, and throws
        /// an exception if more than one such element exists.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>TV object</returns>
        public T Single(Func<T, bool> predicate)
        {
            if (!_disposed)
            {
                _lock.EnterReadLock();
                try
                {
                    return _items.Single(predicate);
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            return default;
        }

        /// <summary>
        /// Returns the only element of a sequence that satisfies a specified condition or a default
        /// value if no such element exists; this method throws an exception if more than one element
        /// satisfies the condition.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>TV object</returns>
        public T SingleOrDefault(Func<T, bool> predicate)
        {
            if (!_disposed)
            {
                _lock.EnterReadLock();
                try
                {
                    return _items.SingleOrDefault(predicate);
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            return default;
        }

        /// <summary>
        /// Returns a number that represents how many elements in the specified sequence satisfy a condition.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns>integer number of found elements</returns>
        public int Sum(Func<T, int> selector)
        {
            if (!_disposed)
            {
                _lock.EnterReadLock();
                try
                {
                    return _items.Sum(selector);
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            return 0;
        }

        /// <summary>
        /// Returns a number that represents how many elements in the specified sequence satisfy a condition.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns>integer number of found elements</returns>
        public int? Sum(Func<T, int?> selector)
        {
            if (!_disposed)
            {
                _lock.EnterReadLock();
                try
                {
                    return _items.Sum(selector);
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            return 0;
        }

        /// <summary>
        /// Returns a number that represents how many elements in the specified sequence satisfy a condition.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns>integer number of found elements</returns>
        public long Sum(Func<T, long> selector)
        {
            if (!_disposed)
            {
                _lock.EnterReadLock();
                try
                {
                    return _items.Sum(selector);
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            return 0;
        }

        /// <summary>
        /// Returns a number that represents how many elements in the specified sequence satisfy a condition.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns>integer number of found elements</returns>
        public long? Sum(Func<T, long?> selector)
        {
            if (!_disposed)
            {
                _lock.EnterReadLock();
                try
                {
                    return _items.Sum(selector);
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            return 0;
        }

        /// <summary>
        /// Returns a number that represents how many elements in the specified sequence satisfy a condition.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns>integer number of found elements</returns>
        public double Sum(Func<T, double> selector)
        {
            if (!_disposed)
            {
                _lock.EnterReadLock();
                try
                {
                    return _items.Sum(selector);
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            return 0;
        }

        /// <summary>
        /// Returns a number that represents how many elements in the specified sequence satisfy a condition.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns>integer number of found elements</returns>
        public double? Sum(Func<T, double?> selector)
        {
            if (!_disposed)
            {
                _lock.EnterReadLock();
                try
                {
                    return _items.Sum(selector);
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            return 0;
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public List<T> Where(Func<T, bool> predicate)
        {
            if (!_disposed)
            {
                _lock.EnterReadLock();
                try
                {
                    return _items.Where(predicate).ToList();
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            return new List<T>();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Clear();
                _lock.Dispose();
            }
        }

        #endregion
    }
}