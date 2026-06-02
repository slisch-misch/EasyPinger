using EasyPinger.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace EasyPinger.Models
{
    internal class Journal
    {
        public void Write(string message)
        {
            var fileName = $"logs_{DateTime.Now.ToShortDateString()}.txt";
            using (FileStream fstream = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                if (fstream.Length == 0)
                {
                    var firstMessage = $"Начало записи - {DateTime.Now.ToString()}\r\n";
                    // преобразуем строку в байты
                    byte[] buffer = Encoding.Default.GetBytes(firstMessage);
                    // запись массива байтов в файл
                    fstream.Write(buffer, 0, buffer.Length);
                    Console.WriteLine("Журнал создан.");
                }
            }
            using (FileStream fstream = new FileStream(fileName, FileMode.Append, FileAccess.Write))
            {
                string messageWithNewLine = message + "\r\n";
                byte[] buffer = Encoding.Default.GetBytes(messageWithNewLine);
                fstream.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
