using Newtonsoft.Json;

namespace EasyPinger.Models
{
    internal class ConfigModel
    {
        public Service[] Services;
        public int TimeOut;
        public NotificationMode Mode;
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

    }

    public class Service
    {
        public Service(string ip, string name)
        {
            InternetAddress = ip;
            DisplayName = name;
        }

        public string InternetAddress { get; set; }
        public string DisplayName { get; set; }
        public int TriesCounter { get; set; }
    }

    enum NotificationMode
    {
        AllMessages,
        ErrorsOnly
    }
}
