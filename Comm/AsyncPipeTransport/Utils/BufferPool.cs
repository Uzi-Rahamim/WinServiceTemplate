using System.Collections.Concurrent;

namespace AsyncPipeTransport.Utils
{ 
    public class BufferPool<T> where T : BaseBufferItem 
    {
        private int _sizeToAllocate = 0;
        private ConcurrentQueue<T> _items = new ConcurrentQueue<T>();
        private Func<Action<BaseBufferItem>, T> _factory;

        public BufferPool(int size, Func<Action<BaseBufferItem>, T> factory)
        {
            _sizeToAllocate = size;
            _factory = factory;
        }

        public void ReturnItem(BaseBufferItem buffer)
        {
            _items.Enqueue((T)buffer);
        }

        public T? Allocate()
        {
            if (_items.TryDequeue(out var buffer))
            {
                return buffer;
            }

            if (Interlocked.Decrement(ref _sizeToAllocate) >= 0)
            {
                var newBuffer = _factory(this.ReturnItem);
                _items.Enqueue(newBuffer);
                return newBuffer;
            }

            return null;
        }
    }
}
