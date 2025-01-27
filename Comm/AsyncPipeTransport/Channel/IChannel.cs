using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncPipeTransport.Channel
{
    public interface IChannel: IChannelSender, IDisposable
    {
        Task<string?> ReceiveAsync();
    }
}
