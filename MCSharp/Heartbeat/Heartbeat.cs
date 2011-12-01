using System.Net;

namespace MCSharp.Heartbeat
{
    public class Heartbeat
    {
        protected HttpWebRequest request;

        protected int _timeout = 20000; // 20 * 1000 = 20 seconds
        public int Timeout { get { return _timeout; } }

        protected string serverURL = "";
        protected string postVars = "";
        protected string staticPostVars = "";

        public Heartbeat ()
        {

        }
    }
}
