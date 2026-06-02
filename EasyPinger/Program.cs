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
                foreach (var service in config.Services)
                {
                    using (Ping ping = new Ping())
                    {
                        try
                        {
                            PingReply reply = ping.Send(service.InternetAddress, 5000);

                            if (reply.Status == IPStatus.Success)
                            {
                                Console.WriteLine($"Успешно: {service.DisplayName} активен. Время ответа: {reply.RoundtripTime} мс");
                            }
                            else
                            {
                                Console.WriteLine($"Не удалось пингануть {service.DisplayName}. Статус: {reply.Status}");
                                badResponse = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Ошибка при пинге {service.DisplayName}: {ex.Message}");
                            badResponse = true;
                        }
                    }
                }
                Thread.Sleep(config.TimeOut);
            }
        }
    }
}
