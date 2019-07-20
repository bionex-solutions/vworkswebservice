using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VWorks4Lib;
using IniParser;
using IniParser.Model;

using Newtonsoft.Json;
using System.IO;
using System.Net;


namespace SimpleVWorksWebService
{
    public class VWorksAPIWrapper
    {
        public VWorksAPIWrapper(VWorks4API vw)
        {

            //vw = new VWorks4API();

            vw.ShowVWorks(true);

            vw.Login(API_CONSTANTS.VWORKS.USER_NAME, API_CONSTANTS.VWORKS.PASSWORD);

            vw.ProtocolComplete += ((int session, string protocol, string protocol_type) =>
            {
                try
                {
                    //--Close protocol when complete

                    if (API_CONSTANTS.EVENT_ENDPOINT_SWITCH.CLOSE_PROTOCOL)
                    {
                        vw.CloseProtocol(protocol);
                    }

                    //--Post to a protocol complete endpoint if turned on
                    if (API_CONSTANTS.EVENT_ENDPOINT_SWITCH.PROTOCOL_COMPLETE)
                    {
                        string protocolName = Path.GetFileName(protocol);
                        Protocol p = new Protocol();

                        p.session = session;
                        p.protocol = protocolName;
                        p.protocol_type = protocol_type;

                        string ProtocolData = JsonConvert.SerializeObject(p);
                        Utilities.Post(ProtocolData, API_CONSTANTS.EVENT_ENDPOINTS.PROTOCOL_COMPLETE);
                   
                    }


                }

                catch (Exception we)
                {
                    Console.WriteLine("Protocol Complete Event Error");
                    Console.WriteLine(we.Message);


                }

            });

            // --TODO: Implement other events as needed.
            /*
            vw.RecoverableError += ((int session, string device, string location, string description, out int actionToTake, out bool vworksHandlesError) =>
            {

            });

            vw.MessageBoxAction += ((int session, int type, string message, string caption, out int actionsToTake) =>
            {

            }
           );*/



        }

        public void VWorksAction(HttpListenerRequest request, VWorks4API vw)
        {
            try
            {   
                payload p = Utilities.GetRequestBody(request);

                switch (p.action)
                {
                    case "run protocol":

                        //--Copy Template Protocol, append ID and add to a working folder, run protocol
                        string fileName = Path.GetFileName(p.path);

                        string file_path = API_CONSTANTS.VWORKS.WORKING_DIRECTORY + p.run_id + "_" + fileName;

                        string text = File.ReadAllText(p.path);

                       //--Inject JSON into VWorks Protocol

                       File.WriteAllText(file_path, text.Replace("process_variables",  p.processVariables.ToString() ));

                        vw.RunProtocol(file_path, p.numTimes);

                        break;

                    case "pause":

                        vw.PauseProtocol();

                        break;

                    case "continue":

                        vw.ResumeProtocol();

                        break;

                    case "abort":

                        vw.AbortProtocol();

                        break;

                    case "hide vworks":

                        vw.ShowVWorks(false);

                        break;
                    case "show vworks":

                        vw.ShowVWorks(true);

                        break;

                    default:
                        Console.WriteLine("Invalid Method Received");
                        Console.WriteLine("Method Does Not Exist");

                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("API Method Exception");
                Console.WriteLine(e.Message);
            }
        }
    }
}
