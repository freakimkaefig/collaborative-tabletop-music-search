using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blake.NUI.WPF.Utility
{
    public abstract class PoolItem<TFormat>
    {
        public TFormat Format { get; private set; }
        
        public PoolItem(TFormat format)
        {
            this.Format = format;
        }
    }

    public class Pool<TItem, TFormat>
        where TItem : PoolItem<TFormat>
    {
        private readonly Func<TFormat, TItem> _factoryFunction;
        private readonly Action<TItem> _resetFunction;
        private readonly Stack<TItem> _pool;

        public int Count
        {
            get
            {
                return _pool.Count;
            }
        }

        private TFormat _format;
        public TFormat Format
        {
            get
            {
                return _format;
            }
            set
            {
                if (_format.Equals(value))
                    return;
                _format = value;
                RecreatePoolItems();
            }
        }

        public int Capacity { get; private set; }

        public Pool(int poolCapacity, TFormat format, Func<TFormat, TItem> factoryFunction, Action<TItem> resetFunction)
        {
            _factoryFunction = factoryFunction;
            _resetFunction = resetFunction;
            _format = format;
            _pool = new Stack<TItem>();
            Capacity = poolCapacity;
            RecreatePoolItems();
        }

        public Pool(int poolCapacity, TFormat format, Func<TFormat, TItem> factoryFunction)
            : this(poolCapacity, format, factoryFunction, null)
        {
        }

        public void RecreatePoolItems()
        {
            lock (_pool)
            {
                _pool.Clear();
                for (var i = 0; i < Capacity; i++)
                {
                    var item = _factoryFunction(Format);
                    if (item == null)
                    {
                        throw new NullReferenceException("factoryFunction returned a null item");
                    }
                    _pool.Push(item);
                }
            }
        }

        public void Push(TItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException();
            }
            if (!item.Format.Equals(this.Format))
            {
                //Returning the wrong type to the pool, 
                //perhaps from before a format change.
                //Can't do anything with it now so throw it away.
                return;
            }
            if (_resetFunction != null)
            {
                _resetFunction(item);
            }
            lock (_pool)
            {
                _pool.Push(item);
            }
        }

        public TItem Pop()
        {
            lock (_pool)
            {
                if (_pool.Count == 0)
                {
                    return null;
                }
                var item = _pool.Pop();
                return item;
            }
        }
    }
}
