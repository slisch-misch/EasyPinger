using System.Text;

namespace EasyPinger.Models
{
    internal static class Journal
    {
        public static async Task Write(string message)
        {
            var fileName = $"logs_{DateTime.Now.ToShortDateString()}.txt";
            await using (FileStream fstream = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                if (fstream.Length == 0)
                {
                    var firstMessage = $"Начало записи - {DateTime.Now.ToString()}{Environment.NewLine}";
                    // преобразуем строку в байты
                    byte[] buffer = Encoding.Default.GetBytes(firstMessage);
                    // запись массива байтов в файл
                    await fstream.WriteAsync(buffer, 0, buffer.Length);
                    Console.WriteLine("Журнал создан.");
                }
            }

            await using (FileStream fstream = new FileStream(fileName, FileMode.Append, FileAccess.Write))
            {
                Console.WriteLine("Журнал найден.");
                string messageWithNewLine = message + Environment.NewLine;
                byte[] buffer = Encoding.Default.GetBytes(messageWithNewLine);
                await fstream.WriteAsync(buffer, 0, buffer.Length);
            }
        }
    }
}
