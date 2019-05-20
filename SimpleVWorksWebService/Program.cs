using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VWorks4Lib;
using Newtonsoft.Json;
using System.IO;

namespace SimpleVWorksWebService
{
    class Program
    {
        private static VWorks4API vw;
        private static VWorksAPIWrapper api;
        static void Main(string[] args)
        {


            vw = new VWorks4API();
            api = new VWorksAPIWrapper(vw);
            WebServer ws = new WebServer(SendResponse, API_CONSTANTS.ENDPOINT_SETTINGS.URL);

            ws.Run();
            Console.WriteLine("A simple webserver. Press a key to quit.");
            Console.ReadKey();
            ws.Stop();
        }

        public static string SendResponse(HttpListenerRequest request)
        {
            try
            {
                api.VWorksAction(request, vw);
              
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);

            }
            return "";
        }
    }
}
