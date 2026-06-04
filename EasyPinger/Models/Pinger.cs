using MailKit.Security;
using MimeKit;
using MailKit.Net.Smtp;
using EasyPinger.Helper;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using MailKit;

namespace EasyPinger.Models
{
    internal class Pinger()
    {
        private static readonly HttpClient HttpClient = new();

        public static Dictionary<string, int> servicesDownStatistics = [];

        public async Task PingAsync()
        {           
            while (true)
            {
                var config = await ConfigGenerator.GetConfigAsync();
                

                foreach (var service in config.Services)
                {                    
                    try
                    {
                        using var result = await HttpClient.GetAsync(service.InternetAddress);

                        if (result.IsSuccessStatusCode)
                        {
                            servicesDownStatistics.Remove(service.InternetAddress);

                            if (config.NotificationMode == NotificationMode.AllMessages)
                                await Journal.Write($"Успешно: {service.DisplayName} активен.");
                        }
                        else
                        {
                            await Journal.Write(
                                $"{DateTime.Now} - Не удалось пингануть {service.DisplayName}. Статус: {result.StatusCode}:{result.ReasonPhrase}");
                            AddOrUpdate(service);
                        }
                    }
                    catch (Exception ex)
                    {
                        await Journal.Write($"{DateTime.Now} - Ошибка при пинге {service.DisplayName}: {ex.Message}");
                        AddOrUpdate(service);
                    }
                }
                var counter = servicesDownStatistics.Count > 0 ? servicesDownStatistics.Max(c => c.Value) : 0;
                if (counter == config.TriesCounter)
                    await SendMail(GetServiceDownStatistics());

                await Task.Delay(config.TimeOut);
            }
        }

        public async Task SendMail(string message)
        {
            var mailConfig = await MailConfigGenerator.GetConfigAsync();

            if (string.IsNullOrEmpty(mailConfig.SenderAddress) || string.IsNullOrEmpty(mailConfig.SenderPassword))
            {
                Console.WriteLine("Отсутствуют учётные данные отправителя");
                return;
            }

            if (mailConfig.Recipients == null || mailConfig.Recipients.Length == 0)
            {
                Console.WriteLine("Не указаны получатели письма");
                return;
            }
            try
            {
                using (var client = new SmtpClient())
                {
                    var smtp = MailConfigGenerator.GetSmtpInfo(mailConfig.SenderAddress);
                    if (!smtp.HasValue)
                    {
                        Console.WriteLine($"SMTP не получен - {mailConfig.SenderAddress}");
                        return;
                    }
                    await client.ConnectAsync(smtp.Value.SmtpHost, smtp.Value.Port, smtp.Value.options);
                    if (!client.IsConnected)
                    {
                        Console.WriteLine($"Не удалось подключиться - {mailConfig.SenderAddress}, {smtp.Value.SmtpHost} - {smtp.Value.Port}");
                        return;
                    }
                    await client.AuthenticateAsync(mailConfig.SenderAddress, mailConfig.SenderPassword);
                    if (!client.IsAuthenticated)
                    {
                        Console.WriteLine($"Не удалось авторизовать почту отправителя - {mailConfig.SenderAddress}");
                        return;
                    }
                    var mail = new MimeMessage();
                    mail.From.Add(new MailboxAddress(mailConfig.SenderName, mailConfig.SenderAddress));


                    mail.Subject = mailConfig.MessageTitle ?? string.Empty;
                    mail.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                    {
                        Text = mailConfig.MessageText + Environment.NewLine + "Текст ошибки: " + message
                    };
                    foreach (var receiver in mailConfig.Recipients)
                    {
                       mail.To.Add(new MailboxAddress(receiver.Email, receiver.Email));
                    }
                    await client.SendAsync(mail);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Не удалось отправить письмо. Текст ошибки: " + ex.Message);
            }                          
        }

        public static void AddOrUpdate(Service service)
        {
            servicesDownStatistics.TryAdd(service.InternetAddress, 0);
            servicesDownStatistics[service.InternetAddress]++;
        }

        public static string GetServiceDownStatistics()
        {
            var logBuilder = new StringBuilder();
            foreach (var service in servicesDownStatistics) 
            {
                logBuilder.AppendLine($"{service} упал {service.Value} раз{Environment.NewLine}");
            }
            return logBuilder.ToString();
        }
    }
}