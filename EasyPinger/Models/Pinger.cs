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

        public Pinger()
        {
            Config = ConfigGenerator.GetConfigAsync().Result;
        }

        public async Task PingAsync()
        {
            while (!BadResponse)
            {
                using (var client = new HttpClient())
                {
                    foreach (var service in Config.Services)
                    {
                        try
                        {
                            using var result = await client.GetAsync(service.InternetAddress);
                            
                            if (result.IsSuccessStatusCode)
                            {
                                if (Config.Mode == NotificationMode.AllMessages)
                                    Journal.Write(($"Успешно: {service.DisplayName} активен."));
                            }
                            else
                            {
                                Journal.Write(($"{DateTime.Now} - Не удалось пингануть {service.DisplayName}. Статус: {result.StatusCode}:{result.ReasonPhrase}"));
                                BadResponse = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            Journal.Write(($"{DateTime.Now} - Ошибка при пинге {service.DisplayName}: {ex.Message}"));
                            BadResponse = true;
                        }
                    }
                }
                Thread.Sleep(Config.TimeOut);
            }
        }
    }
}
