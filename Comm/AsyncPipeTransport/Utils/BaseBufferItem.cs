namespace AsyncPipeTransport.Utils
{
    public abstract class BaseBufferItem : IDisposable
    {
        private Action<BaseBufferItem> _returnItem;
        protected abstract void ResetItem();
        public BaseBufferItem(Action<BaseBufferItem> returnItem)
        {
            _returnItem = returnItem;
        }

        public void Dispose()
        {
            ResetItem();
            _returnItem(this);
        }
    }
}
