using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace MCSharp
{
    public class CmdServerReport : Command
    {
        
        // Constructor
        public CmdServerReport(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }
        
        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/serverreport - Get server CPU%, RAM usage, and uptime.");
        }
        
        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            TimeSpan tp =  Process.GetCurrentProcess().TotalProcessorTime;
            TimeSpan up = (DateTime.Now - Process.GetCurrentProcess().StartTime);
            string ProcessorUsage = "CPU Usage (This only works in windows at the moment!)";
            //To get actual CPU% is OS dependant
            if (System.Environment.OSVersion.Platform != PlatformID.Unix)
            {
                ProcessorUsage = "CPU Usage (Processes : All Processes):" + Server.ProcessCounter.NextValue() + " : " + Server.PCCounter.NextValue();
            }
            //Alternative Average?
            //string ProcessorUsage = "CPU Usage is Not Implemented: So here is ProcessUsageTime/ProcessTotalTime:"+String.Format("00.00",(((tp.Ticks/up.Ticks))*100))+"%";
            //reports Private Bytes because it is what the process has reserved for itself and is unsharable
            string MemoryUsage = "Memory Usage: "+Math.Round((double)Process.GetCurrentProcess().PrivateMemorySize64/1048576).ToString()+" Megabytes";
            string Uptime =  "Uptime: "+up.Days+" Days "+up.Hours+" Hours "+up.Minutes+" Minutes "+up.Seconds+" Seconds";
            p.SendMessage(Uptime);
            p.SendMessage(MemoryUsage);
            p.SendMessage(ProcessorUsage);
        }
    }
}
