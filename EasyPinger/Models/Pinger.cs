using MailKit.Security;
using MimeKit;
using MailKit.Net.Smtp;
using EasyPinger.Helper;

namespace EasyPinger.Models
{
    internal class Pinger(ConfigModel config, MailConfig mailConfig)
    {
        private static readonly HttpClient HttpClient = new();

        public async Task PingAsync()
        {            
            var counter = config.TriesCounter;
            while (counter > 0)
            {
                var unavailableService = new List<Service>();
                foreach (var service in config.Services)
                {
                    try
                    {
                        using var result = await HttpClient.GetAsync(service.InternetAddress);

                        if (result.IsSuccessStatusCode)
                        {
                            unavailableService.Remove(service);
                            if (config.NotificationMode == NotificationMode.AllMessages)
                                await Journal.Write($"Успешно: {service.DisplayName} активен.");
                        }
                        else
                        {
                            await Journal.Write(
                                $"{DateTime.Now} - Не удалось пингануть {service.DisplayName}. Статус: {result.StatusCode}:{result.ReasonPhrase}");
                            unavailableService.Add(service);
                            counter--;
                        }
                    }
                    catch (Exception ex)
                    {
                        await Journal.Write($"{DateTime.Now} - Ошибка при пинге {service.DisplayName}: {ex.Message}");
                        unavailableService.Add(service);
                        counter--;
                    }
                }
                if (counter == 0)
                    await SendMail(string.Join(Environment.NewLine, unavailableService.Select(c => c.DisplayName)));

                if (config.IgnoreCounterMode == IgnoreCounterMode.Ignore)
                    counter = config.TriesCounter;

                await Task.Delay(config.TimeOut);
            }
        }

        public async Task SendMail(string message)
        {
            if (mailConfig == null)
            {
                Console.WriteLine("Отсутствует mail-конфигурация");
                return;
            }

            if (string.IsNullOrEmpty(mailConfig.SenderAddress) || string.IsNullOrEmpty(mailConfig.SenderPassword))
            {
                Console.WriteLine("Отсутствует учётные данные отправителя");
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
                    if (smtp == null || !smtp.HasValue)
                    {
                        Console.WriteLine($"SMTP не получен - {mailConfig.SenderAddress}");
                        return;
                    }
                    await client.ConnectAsync(smtp.Value.SmtpHost, smtp.Value.Port, SecureSocketOptions.StartTlsWhenAvailable);
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
    }
}