namespace Intel.IntelConnect.IPC.Utils.BufferAllocation
{
    public class BytesBufferItem : BaseBufferItem
    {
        public byte[] Buffer { get; private set; }
        public int Length { get; set; } = 0;
        public int Offset { get; set; } = 0;

        public BytesBufferItem(int size, Action<BaseBufferItem> returnItem) : base(returnItem)
        {
            Buffer = new byte[size];
        }

        protected override void ResetItem()
        {
            Length = 0;
            Offset = 0;
        }
    }

}
