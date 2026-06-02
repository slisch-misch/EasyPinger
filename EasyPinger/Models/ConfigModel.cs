using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EasyPinger.Models
{
    internal class ConfigModel
    {
        public Service[] Services;
        public int TimeOut;
        public NotificationMode Mode;
        public int TriesCounter { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
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
    }

    enum NotificationMode
    {
        AllMessages,
        ErrorsOnly
    }
}
