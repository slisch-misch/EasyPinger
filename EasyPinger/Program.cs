using EasyPinger.Models;
using System.Text;
using Newtonsoft.Json;
using EasyPinger.Helper;
using System.Net.NetworkInformation;
using System.Threading;

namespace EasyPinger
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigGenerator.GetConfig();
            var badResponse = false;
            while (!badResponse)
            {
                var journal = new Journal();
                foreach (var service in config.Services)
                {
                    using (Ping ping = new Ping())
                    {
                        try
                        {
                            PingReply reply = ping.Send(service.InternetAddress, 5000);

                            if (reply.Status == IPStatus.Success)
                            {
                                journal.Write(($"Успешно: {service.DisplayName} активен. Время ответа: {reply.RoundtripTime} мс"));
                            }
                            else
                            {
                                journal.Write(($"Не удалось пингануть {service.DisplayName}. Статус: {reply.Status}"));
                                badResponse = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            journal.Write(($"Ошибка при пинге {service.DisplayName}: {ex.Message}"));
                            badResponse = true;
                        }
                    }
                }
                Thread.Sleep(config.TimeOut);
            }
        }
    }
}
