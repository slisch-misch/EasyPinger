namespace EasyPinger.Models
{
    internal class Pinger(ConfigModel config)
    {
        private static readonly HttpClient HttpClient = new();

        public async Task PingAsync()
        {
            var badResponse = false;

            while (!badResponse)
            {
                foreach (var service in config.Services)
                {
                    try
                    {
                        using var result = await HttpClient.GetAsync(service.InternetAddress);

                        if (result.IsSuccessStatusCode)
                        {
                            if (config.Mode == NotificationMode.AllMessages)
                                await Journal.Write($"Успешно: {service.DisplayName} активен.");
                        }
                        else
                        {
                            await Journal.Write(
                                $"{DateTime.Now} - Не удалось пингануть {service.DisplayName}. Статус: {result.StatusCode}:{result.ReasonPhrase}");
                            badResponse = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        await Journal.Write($"{DateTime.Now} - Ошибка при пинге {service.DisplayName}: {ex.Message}");
                        badResponse = true;
                    }
                }

                await Task.Delay(config.TimeOut);
            }
        }
    }
}