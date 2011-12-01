using System;
using System.Collections.Generic;
using System.Text;

namespace MCSharp
{
    public class Report
    {
        string name, ip, reason;

        public string Name { get { return name; } }
        public string IP { get { return ip; } }
        public string Reason { get { return reason; } }

        public Report(Player p, string reason)
        {
            name = p.name;
            ip = p.ip;
            this.reason = reason;
        }
    }
}
