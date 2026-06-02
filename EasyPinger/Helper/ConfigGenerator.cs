using EasyPinger.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace EasyPinger.Helper
{
    internal static class ConfigGenerator
    {
        public static ConfigModel Generate()
        {
            var config = new ConfigModel();
            config.TimeOut = 60000;
            config.Mode = NotificationMode.AllMessages;
            config.Services = new Service[2];
            config.TriesCounter = 3;
            var yandex = new Service("https://ya.ru/", "Яндекс");
            config.Services[0] = yandex;
            var google = new Service("https://www.google.com/", "Гугл");
            config.Services[1] = google;
            return config;
        }

        

        public static async Task<ConfigModel> GetConfigAsync()
        {
            await using FileStream fstream = new FileStream("config.txt", FileMode.OpenOrCreate);
            if (fstream.Length == 0)
            {
                var config = Generate();
                Console.WriteLine("Конфигурация не найдена.");

                // преобразуем строку в байты
                byte[] buffer = Encoding.Default.GetBytes(config.ToString());
                // запись массива байтов в файл
                await fstream.WriteAsync(buffer, 0, buffer.Length);
                Console.WriteLine("Конфигурация создана.");
                return config;
            }
            else
            {
                // выделяем массив для считывания данных из файла
                byte[] buffer = new byte[fstream.Length];
                // считываем данные
                fstream.Read(buffer, 0, buffer.Length);
                // декодируем байты в строку
                var textFromFile = Encoding.Default.GetString(buffer);
                var config = JsonConvert.DeserializeObject<ConfigModel>(textFromFile);
                Console.WriteLine($"Конфигурация получена.");
                return config;
            }
        }
    }
}
