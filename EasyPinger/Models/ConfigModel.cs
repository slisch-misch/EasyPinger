using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json; 

namespace EasyPinger.Models
{
    internal class ConfigModel
    {
        public Service[] Services;
        public int TimeOut;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public void Generate()
        {
            this.TimeOut = 3600;
            this.Services = new Service[1];
            this.Services[0].DisplayName = "Яндекс";
            this.Services[0].InternetAddress = "ya.ru";
        }
    }

    public class Service
    {
        public Service (string ip, string name, int tries)
        {
            InternetAddress = ip;
            DisplayName = name;
            TriesCounter = tries;
        }

        public string InternetAddress { get; set; }
        public string DisplayName { get; set; }
        public int TriesCounter { get; set; }
    }
}
