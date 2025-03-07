﻿using Intel.IntelConnect.IPC.CommonTypes;
namespace PluginB.Contract.Massages
{
    public partial class MethodName
    {
        public const string PluginB_Echo = "PluginB.Echo";
    }

    public class RequestEchoMessage : MessageHeader
    {
        public string message { get; set; }

        public RequestEchoMessage(string message)  => this.message = message;
    }

    public class ResponseEchoMessage : MessageHeader
    {
        public string message { get; set; }
        public ResponseEchoMessage(string message)  => this.message = message;
    }

}