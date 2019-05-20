using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleVWorksWebService
{
    class VWorksObjects
    {
        public string ObjectType { get; set; }
        public Log VWorksLog { get; set; }
        public Protocol VWorksProtocol { get; set; }
        public Error VWorksError { get; set; }
    }
    public class Log
    {

        public int session { get; set; }
        public int logClass { get; set; }
        public string timeStamp { get; set; }
        public string device { get; set; }
        public string location { get; set; }
        public string process { get; set; }
        public string task { get; set; }
        public string filename { get; set; }
        public string message { get; set; }

    }
    public class Protocol
    {
        public int session { get; set; }
        public string protocol { get; set; }
        public string protocol_type { get; set; }

    }

    //int session, string device, string location, string description, out int actionToTake, out bool vworksHandlesError
    public class Error
    {
        public int session { get; set; }
        public string device { get; set; }
        public string location { get; set; }

        public string description { get; set; }


    }
}
