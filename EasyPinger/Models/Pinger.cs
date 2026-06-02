using EasyPinger.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPinger.Models
{
    internal class Pinger
    {
        ConfigModel Config { get; set; }
        bool BadResponse = false;

        public async Task PingAsync()
        {
            var config = ConfigGenerator.GetConfigAsync().Result;
            while (!BadResponse)
            {
                using (var client = new HttpClient())
                {
                    foreach (var service in config.Services)
                    {
                        try
                        {
                            using var result = await client.GetAsync(service.InternetAddress);

                            if (result.StatusCode == System.Net.HttpStatusCode.OK && config.Mode == NotificationMode.AllMessages)
                            {
                                Journal.Write(($"Успешно: {service.DisplayName} активен."));
                            }
                            else
                            {
                                Journal.Write(($"Не удалось пингануть {service.DisplayName}. Статус: {result.StatusCode}:{result.ReasonPhrase}"));
                                BadResponse = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            Journal.Write(($"Ошибка при пинге {service.DisplayName}: {ex.Message}"));
                            BadResponse = true;
                        }
                    }
                }
                Thread.Sleep(config.TimeOut);
            }
        }
    }
}
