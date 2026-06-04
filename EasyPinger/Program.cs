using EasyPinger.Helper;
using EasyPinger.Models;

namespace EasyPinger
{
    internal class Program
    {
        static async Task Main()
        {            
            var pinger = new Pinger();
            await pinger.PingAsync();
        }
    }
}