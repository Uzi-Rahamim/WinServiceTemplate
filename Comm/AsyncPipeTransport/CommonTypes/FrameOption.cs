namespace AsyncPipeTransport.CommonTypes
{
    [Flags]
    public enum FrameOptions : int
    {
        None = 0x00,
        LastFrame = 0x01,
        OpenSession = 0x02,
        Discovery = 0x04,

        Request = 0x10,
        Response = 0x20,
        Pulse = 0x40 | LastFrame,
        EvantMsg = 0x80 | LastFrame
    }
}
