namespace Intel.IntelConnect.IPC.CommonTypes
{
    [Flags]
    public enum FrameOptions : int
    {
        None = 0x00,
        LastFrame = 0x01,
        
        RequestMsg = 0x10,
        ResponseMsg = 0x20,
        PulseMsg = 0x40 | LastFrame,
        EvantMsg = 0x80 | LastFrame,
        ErrorMsg = 0x100 | LastFrame,
        OpenSessionMsg = 0x200,
    }
}
