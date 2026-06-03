using EasyPinger.Models;
using MailKit.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPinger.Helper
{
    internal class MailConfigGenerator
    {
        public static MailConfig Generate()
        {
            var config = new MailConfig
            {
                SenderAddress = "test@yandex.ru",
                SenderPassword = "password",
                SenderName = "Пингатор",
                MessageTitle = "Сервисы недоступны",
                MessageText = "Сервисы недоступны, см. логи",
                Recipients = [new Recipient("test@gmail.com"), new Recipient("test@mail.ru")]
            };
            return config;
        }

        public static async Task<MailConfig> GetConfigAsync()
        {
            await using FileStream fstream = new("mailconfig.txt", FileMode.OpenOrCreate);
            if (fstream.Length == 0)
            {
                var config = Generate();
                Console.WriteLine("Почтовая конфигурация не найдена.");

                // преобразуем строку в байты
                byte[] buffer = Encoding.Default.GetBytes(config.ToString());
                // запись массива байтов в файл
                await fstream.WriteAsync(buffer, 0, buffer.Length);
                Console.WriteLine("Почтовая конфигурация создана.");
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
                var config = JsonConvert.DeserializeObject<MailConfig>(textFromFile);
                Console.WriteLine($"Почтовая конфигурация получена.");
                return config;
            }
        }

        private static readonly Dictionary<string, (string SmtpHost, int Port, SecureSocketOptions options)> SmtpMap =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["gmail.com"] = ("smtp.gmail.com", 587, SecureSocketOptions.StartTls),
            ["yandex.ru"] = ("smtp.yandex.ru", 465, SecureSocketOptions.SslOnConnect),
            ["ya.ru"] = ("smtp.yandex.ru", 465, SecureSocketOptions.SslOnConnect),
            ["mail.ru"] = ("smtp.mail.ru", 465, SecureSocketOptions.SslOnConnect),
            ["inbox.ru"] = ("smtp.mail.ru", 465, SecureSocketOptions.SslOnConnect),
            ["list.ru"] = ("smtp.mail.ru", 465, SecureSocketOptions.SslOnConnect),
            ["bk.ru"] = ("smtp.mail.ru", 465, SecureSocketOptions.SslOnConnect),
            ["outlook.com"] = ("smtp-mail.outlook.com", 587, SecureSocketOptions.StartTls),
            ["hotmail.com"] = ("smtp-mail.outlook.com", 587, SecureSocketOptions.StartTls),
            ["live.com"] = ("smtp-mail.outlook.com", 587, SecureSocketOptions.StartTls),
            ["yahoo.com"] = ("smtp.mail.yahoo.com", 465, SecureSocketOptions.SslOnConnect),
            ["icloud.com"] = ("smtp.mail.me.com", 587, SecureSocketOptions.StartTls),
            ["rambler.ru"] = ("smtp.rambler.ru", 465, SecureSocketOptions.SslOnConnect),
            ["aol.com"] = ("smtp.aol.com", 465, SecureSocketOptions.SslOnConnect),
        };

        public static (string SmtpHost, int Port, SecureSocketOptions options)? GetSmtpInfo(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            // Извлечение домена после '@'
            int atIndex = email.LastIndexOf('@');
            if (atIndex == -1 || atIndex == email.Length - 1)
                return null;

            string domain = email.Substring(atIndex + 1).ToLowerInvariant();

            // Поиск в словаре
            if (SmtpMap.TryGetValue(domain, out var smtpInfo))
                return smtpInfo;

            return null;
        }
    }
}
