using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading.Tasks;

using System.Threading;
using Ninja.WebSockets;
using System.Collections.Generic;
using System.Text;
using VWorks4Lib;

using Newtonsoft.Json;


namespace SimpleVWorksWebService
{
    public class WebSocketServer : IDisposable
    {
        private TcpListener _listener;
        private bool _isDisposed = false;
        
  



       
        private readonly IWebSocketServerFactory _webSocketServerFactory;
        //private readonly ILoggerFactory _loggerFactory;
        private readonly HashSet<string> _supportedSubProtocols;
        // const int BUFFER_SIZE = 1 * 1024 * 1024 * 1024; // 1GB
        const int BUFFER_SIZE = 4 * 1024 * 1024; // 4MB

        public WebSocketServer(IWebSocketServerFactory webSocketServerFactory, VWorks4API vw, IList<string> supportedSubProtocols = null)
        {
            //_logger = loggerFactory.CreateLogger<WebServer>();
            _webSocketServerFactory = webSocketServerFactory;
            //_loggerFactory = loggerFactory;
            _supportedSubProtocols = new HashSet<string>(supportedSubProtocols ?? new string[0]);

        }

        private void ProcessTcpClient(TcpClient tcpClient, VWorks4API vw)
        {
           
            Task.Run(() => ProcessTcpClientAsync(tcpClient, vw));



        }



        private string GetSubProtocol(IList<string> requestedSubProtocols)
        {
            foreach (string subProtocol in requestedSubProtocols)
            {
                // match the first sub protocol that we support (the client should pass the most preferable sub protocols first)
                if (_supportedSubProtocols.Contains(subProtocol))
                {
                    //_logger.LogInformation($"Http header has requested sub protocol {subProtocol} which is supported");
                    Console.WriteLine($"Http header has requested sub protocol {subProtocol} which is supported");
                    return subProtocol;
                }
            }

            if (requestedSubProtocols.Count > 0)
            {
                Console.WriteLine($"Http header has requested the following sub protocols: {string.Join(", ", requestedSubProtocols)}. There are no supported protocols configured that match.");
            }

            return null;
        }

        private async Task ProcessTcpClientAsync(TcpClient tcpClient, VWorks4API vw)
        {
            CancellationTokenSource source = new CancellationTokenSource();

            try
            {
                if (_isDisposed)
                {
                    return;
                }

                // this worker thread stays alive until either of the following happens:
                // Client sends a close conection request OR
                // An unhandled exception is thrown OR
                // The server is disposed
                Console.WriteLine("Server: Connection opened. Reading Http header from stream");

                // get a secure or insecure stream
                Stream stream = tcpClient.GetStream();
                WebSocketHttpContext context = await _webSocketServerFactory.ReadHttpHeaderFromStreamAsync(stream);

                if (context.IsWebSocketRequest)
                {
                    string subProtocol = GetSubProtocol(context.WebSocketRequestedProtocols);
                    var options = new WebSocketServerOptions() { KeepAliveInterval = TimeSpan.FromSeconds(30), SubProtocol = subProtocol };
                    Console.WriteLine("Http header has requested an upgrade to Web Socket protocol. Negotiating Web Socket handshake");

                    WebSocket webSocket = await _webSocketServerFactory.AcceptWebSocketAsync(context, options);

                    Console.WriteLine("Web Socket handshake response sent. Stream ready.");
                    await RespondToWebSocketRequestAsync(webSocket, source.Token, vw);


                }
                else
                {
                    Console.WriteLine("Http header contains no web socket upgrade request. Ignoring");
                }

                Console.WriteLine("Server: Connection closed");
            }
            catch (ObjectDisposedException)
            {
                // do nothing. This will be thrown if the Listener has been stopped
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                try
                {
                    tcpClient.Client.Close();
                    tcpClient.Close();
                    source.Cancel();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to close TCP connection: {ex}");
                }
            }
        }

        public async Task RespondToWebSocketRequestAsync(WebSocket webSocket, CancellationToken token, VWorks4API vw)
        {
            ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[BUFFER_SIZE]);

            while (true)
            {
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(buffer, token);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    Console.WriteLine($"Client initiated close. Status: {result.CloseStatus} Description: {result.CloseStatusDescription}");
                    break;
                }

                if (result.Count > BUFFER_SIZE)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.MessageTooBig,
                        $"Web socket frame cannot exceed buffer size of {BUFFER_SIZE:#,##0} bytes. Send multiple frames instead.",
                        token);
                    break;
                }

                if (result.MessageType == WebSocketMessageType.Text)
                {
       

                    string value = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);
                    Console.WriteLine(value);


                }

                // just echo the message back to the client
                ArraySegment<byte> toSend = new ArraySegment<byte>(buffer.Array, buffer.Offset, result.Count);
                await webSocket.SendAsync(toSend, WebSocketMessageType.Text, true, token);

                //vw = new VWorks4API();


                vw.LogMessage += ((int session, int logClass, string timeStamp, string device, string location, string process, string task, string fileName, string message) =>
                {
                    VWorksObjects vo = new VWorksObjects();
                    Log vl = new Log();

                    vl.session = session;
                    vl.logClass = logClass;
                    vl.timeStamp = timeStamp;
                    vl.device = device;
                    vl.location = location;
                    vl.process = process;
                    vl.task = task;
                    vl.filename = fileName;
                    vl.message = message;

                    vo.ObjectType = "LogEntry";
                    vo.VWorksLog = vl;


                    //Console.WriteLine("Writing a message");
                    string logString = JsonConvert.SerializeObject(vo);
                    byte[] mybyte = Encoding.ASCII.GetBytes(logString);

                    ArraySegment<byte> plaintext = new ArraySegment<byte>(mybyte, 0, mybyte.Length);

                    webSocket.SendAsync(plaintext, WebSocketMessageType.Text, true, token);



                });

            }
        }


        public async Task Listen(string url, int port, VWorks4API vw)
        {
            try
            {
                IPAddress localAddress = IPAddress.Parse(url); //  IPAddress.Any;
                _listener = new TcpListener(localAddress, port);
                _listener.Start();
                Console.WriteLine($"Server started listening on port {port}");
                while (true)
                {
                    TcpClient tcpClient = await _listener.AcceptTcpClientAsync();
                    ProcessTcpClient(tcpClient, vw);

                }



            }
            catch (SocketException ex)
            {
                string message = string.Format("Error listening on port {0}. Make sure IIS or another application is not running and consuming your port.", port);
                throw new Exception(message, ex);
            }
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;

                // safely attempt to shut down the listener
                try
                {
                    if (_listener != null)
                    {
                        if (_listener.Server != null)
                        {
                            _listener.Server.Close();
                        }

                        _listener.Stop();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                Console.WriteLine("Web Server disposed");
            }
        }
    }
}