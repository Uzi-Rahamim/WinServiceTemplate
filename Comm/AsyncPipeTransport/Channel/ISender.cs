using System.Threading.Tasks;


namespace AsyncPipeTransport.Channel
{
    public interface ISender
    {
        public Task SendAsync(string message);
    }
}
