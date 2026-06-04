using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EasyPinger.Models
{
    internal class ConfigModel
    {
        public Service[] Services;
        public int TimeOut;
        public NotificationMode NotificationMode;
        public int TriesCounter { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }

    public class Service(string ip, string name)
    {
        public string InternetAddress { get; set; } = ip;
        public string DisplayName { get; set; } = name;
    }

    enum NotificationMode
    {
        AllMessages,
        ErrorsOnly
    }
}
