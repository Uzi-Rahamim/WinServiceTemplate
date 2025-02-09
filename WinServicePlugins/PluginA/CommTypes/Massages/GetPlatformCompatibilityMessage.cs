using AsyncPipeTransport.CommonTypes;
namespace Service_48_ExecuterPlugin.CommTypes.Massages
{
    public partial class MessageType
    {
        public static readonly string GetPlatformCompatibility = "GetPlatformCompatibility";
    }

    public class RequestGetPlatformCompatibilityMessage : MessageHeader
    {
        public RequestGetPlatformCompatibilityMessage() :
            base(MessageType.GetPlatformCompatibility) { }
    }

    public class ResponseGetPlatformCompatibilityMessage : MessageHeader
    {
        public IEnumerable<string> list { get; set; }
        public ResponseGetPlatformCompatibilityMessage(IEnumerable<string> list) : 
            base(MessageType.GetPlatformCompatibility) => this.list = list;
    }

}