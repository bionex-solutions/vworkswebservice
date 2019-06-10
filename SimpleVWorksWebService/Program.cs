using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VWorks4Lib;
using Newtonsoft.Json;
using System.IO;

using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Net.WebSockets;
using Ninja.WebSockets;



namespace SimpleVWorksWebService
{
    class Program
    {
        private static VWorks4API vw;
        private static VWorksAPIWrapper apiWrapper;

        static IWebSocketServerFactory _webSocketServerFactory;
        static void Main(string[] args)
        {


            File.Delete(@"C:\Windows\inf\vlm.inf");
            File.Delete(@"C:\Windows\inf\vlm2.inf");

            vw = new VWorks4API();

            apiWrapper = new VWorksAPIWrapper(vw);

            Console.WriteLine(API_CONSTANTS.ENDPOINT_SETTINGS.URL);

            WebServer ws = new WebServer(SendResponse, API_CONSTANTS.ENDPOINT_SETTINGS.URL);



            ws.Run();

            _webSocketServerFactory = new WebSocketServerFactory();

            Task task = StartWebSocketServer(vw);

            task.Wait();

            Console.WriteLine("A client connected.");
            Console.WriteLine("A simple webserver. Press a key to quit.");
            Console.ReadKey();
            ws.Stop();



        }

        static async Task StartWebSocketServer(VWorks4API vw)
        {
            try
            {
                
                IList<string> supportedSubProtocols = new string[] { "chatV1", "chatV2", "chatV3" };

                using (WebSocketServer server = new WebSocketServer(_webSocketServerFactory,vw, supportedSubProtocols))
                {
                    await server.Listen(API_CONSTANTS.WEB_SOCKET_SETTINGS.URL, API_CONSTANTS.WEB_SOCKET_SETTINGS.PORT, vw);
                    Console.WriteLine($"Listening on port {API_CONSTANTS.WEB_SOCKET_SETTINGS.PORT}");
                    Console.WriteLine("Press any key to quit");
                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadKey();
            }
        }

        public static string SendResponse(HttpListenerRequest request)
        {
            try
            {
                apiWrapper.VWorksAction(request, vw);
               

            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);

            }
            return "";
        }
    }
}
