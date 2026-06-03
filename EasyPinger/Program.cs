using EasyPinger.Helper;
using EasyPinger.Models;

namespace EasyPinger
{
    internal class Program
    {
        static async Task Main()
        {
            var config = await ConfigGenerator.GetConfigAsync();
            var mailConfig = await MailConfigGenerator.GetConfigAsync();
            var pinger = new Pinger(config, mailConfig);
            await pinger.PingAsync();
        }
    }
}