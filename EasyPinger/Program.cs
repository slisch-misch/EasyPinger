using EasyPinger.Models;
using System.Text;
using Newtonsoft.Json;
using EasyPinger.Helper;
using System.Net.NetworkInformation;
using System.Net.Http;
using System.Threading;


namespace EasyPinger
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var ping = new Pinger();
            ping.PingAsync().Wait();
        }
    }
}