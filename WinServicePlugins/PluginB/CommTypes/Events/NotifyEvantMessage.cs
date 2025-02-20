﻿using AsyncPipeTransport.CommonTypes;

namespace PluginB.Contract.Massages
{
    public partial class MessageType
    {
        public static readonly string NotifyEvant = "NotifyEvant";
    }

    public class NotifyEvantMessage : MessageHeader
    {
        public string message { get; set; }

        public NotifyEvantMessage(string message) : base(MessageType.NotifyEvant) => this.message = message;
    }

}