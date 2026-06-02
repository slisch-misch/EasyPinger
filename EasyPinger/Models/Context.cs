using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyPinger.Helper;

namespace EasyPinger.Models
{
    internal class Context
    {
        public ConfigModel ConfigModel { get; set; }
        public NotificationMode NotificationMode { get; set; }
    }

    enum NotificationMode
    {
        AllNotifications,
        ErrorsOnly
    }
}
