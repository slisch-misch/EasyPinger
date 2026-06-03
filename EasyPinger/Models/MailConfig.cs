using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPinger.Models
{
    internal class MailConfig
    {
        public string SenderAddress { get; set; }
        public string SenderName { get; set; }
        public string SenderPassword { get; set; }

        public Recipient[] Recipients { get; set; }
        public string MessageText { get; set; }
        public string MessageTitle { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }

    internal class Recipient(string email)
    {
        public string Email { get; set; } = email;

    }
}
