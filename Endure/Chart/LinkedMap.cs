using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Documents;

namespace Endure
{
    public class LinkedMap<TKey, TValue>
    {
        public LinkedMap() { Initialized = false; }

        readonly Dictionary<TKey, TValue> pairs = new Dictionary<TKey, TValue>();
        readonly Dictionary<TKey, Pair<TKey, TKey>> keys = new Dictionary<TKey, Pair<TKey, TKey>>();
        public bool Initialized { get; private set; }
        private TKey PreviusKey { get; set; }
        private TKey NextKey { get; set; }

        public TValue this[TKey key]
        {
            get => pairs[key];
            set => pairs[key] = value;
        }

        public TKey[] Keys { get => pairs.Keys.ToArray<TKey>(); }

        public TKey Tail { get; private set; }
        public TKey Head { get; private set; }

        public bool IsTail(TKey key)
        {
            if (key == null)
                return false;

            return keys[key].First == null;
        }

        public bool IsHead(TKey key)
        {
            if (key == null)
                return false;

            return keys[key].Second == null;
        }

        public TKey GetNext(TKey key)
        {
            return keys[key].Second;
        }

        public TKey GetPrevius(TKey key)
        {
            return keys[key].First;
        }

        public void Initialize(TKey key, TValue value)
        {
            pairs.Add(key, value);
            Head = Tail = key;
            NextKey = PreviusKey = key;
            keys.Add(key, new Pair<TKey, TKey>());
            Initialized = true;
        }

        public void Add(TKey key, TValue value)
        {
            if (keys.ContainsKey(key))
            {

            }
            else
            {
                pairs.Add(key, value);

                if (Initialized)
                {
                    if (keys.ContainsKey(PreviusKey))
                    {
                        keys[PreviusKey].Second = key;
                    }

                    Head = key;
                    PreviusKey = NextKey;

                    keys.Add(key, new Pair<TKey, TKey>() { First = PreviusKey });
                    NextKey = key;
                }
                else
                {
                    Head = Tail = key;
                    NextKey = PreviusKey = key;
                    keys.Add(key, new Pair<TKey, TKey>());
                    Initialized = true;
                }
            }
        }

        public void Remove(TKey key)
        {
            if(keys.ContainsKey(key))
            {
                if(IsTail(key) && IsHead(key))
                {
                    Head = keys[key].First;
                    Tail = keys[key].Second;
                }
                else if (IsHead(key))
                {
                    keys[keys[key].First].Second = keys[key].Second;
                    Head = keys[key].First;
                }
                else if(IsTail(key))
                {
                    keys[keys[key].Second].First = keys[key].First;
                    Tail = keys[key].Second;
                }
                else
                {
                    keys[keys[key].First].Second = keys[key].Second;
                    keys[keys[key].Second].First = keys[key].First;
                }

                keys.Remove(key);
                pairs.Remove(key);
            }
        }

        public void InsertHead(TKey key, TValue value)
        {
            pairs.Add(key, value);
            keys.Add(key, new Pair<TKey, TKey>() { First = Head });

            if (keys.ContainsKey(Head))
            {
                keys[Head].Second = key;
            }

            Head = key;
        }

        public void InsertTail(TKey key, TValue value)
        {
            pairs.Add(key, value);
            keys.Add(key, new Pair<TKey, TKey>() { Second = Tail });

            if (keys.ContainsKey(Tail))
            {
                keys[Tail].First = key;
            }

            Tail = key;
        }

        /// <summary>
        /// current key will be plased infront of origin. Will return false if origin does not exist
        /// </summary>
        public bool InsertFront(TKey origin, TKey currentKey, TValue value)
        {
            if(keys.ContainsKey(origin))
            {
                if (keys.ContainsKey(currentKey))
                {
                    if (origin.Equals(Head))
                    {
                        keys[currentKey].First = origin;
                        keys[currentKey].Second = currentKey;
                        keys[origin].Second = currentKey;
                    }
                    else
                    {
                        keys[currentKey].First = origin;
                        keys[currentKey].Second = keys[origin].Second;

                        if(keys.ContainsKey(keys[origin].Second))
                        {
                            keys[keys[origin].Second].First = currentKey;
                        }

                        keys[origin].Second = currentKey;
                    }
                }
                else
                {
                    if(origin.Equals(Head))
                    {
                        pairs.Add(currentKey, value);
                        keys.Add(currentKey, new Pair<TKey, TKey>() { First = origin });
                        
                        keys[origin].Second = currentKey;
                        Head = currentKey;
                    }
                    else
                    {
                        pairs.Add(currentKey, value);
                        keys.Add(currentKey, new Pair<TKey, TKey>(origin, keys[origin].Second));

                        if (keys.ContainsKey(keys[origin].Second))
                        {
                            keys[keys[origin].Second].First = currentKey;
                        }

                        keys[origin].Second = currentKey;
                    }
                }

                return true;
            }

            return false;
        }
        // current key will be plased behind of origin. Will return false if origin does not exist
        public bool InsertBack(TKey origin, TKey currentKey, TValue value)
        {
            if (keys.ContainsKey(origin))
            {
                if (keys.ContainsKey(currentKey))
                {
                    /// to do make sure previus and next gets replased

                    if (keys[origin].First.Equals(origin))
                    {
                        keys[origin].First = currentKey;
                        keys[currentKey].First = currentKey;
                        keys[currentKey].Second = origin;
                    }
                    else
                    {
                        keys[currentKey].First = keys[origin].First;
                        keys[currentKey].Second = origin;

                        if (keys.ContainsKey(keys[origin].First))
                        {
                            keys[keys[origin].First].Second = currentKey;
                        }

                        keys[origin].First = currentKey;
                    }
                }
                else
                {
                    if (origin.Equals(Tail))
                    {
                        pairs.Add(currentKey, value);
                        keys.Add(currentKey, new Pair<TKey, TKey>() { Second = origin });

                        keys[origin].First = currentKey;
                        Tail = currentKey;
                    }
                    else
                    {
                        pairs.Add(currentKey, value);
                        keys.Add(currentKey, new Pair<TKey, TKey>(keys[origin].First, origin));

                        if (keys.ContainsKey(keys[origin].First))
                        {
                            keys[keys[origin].First].Second = currentKey;
                        }

                        keys[origin].First = currentKey;
                    }
                }

                return true;
            }
            return false;
        }

        public bool ContainsKey(TKey key) { return pairs.ContainsKey(key); }
    }
}
