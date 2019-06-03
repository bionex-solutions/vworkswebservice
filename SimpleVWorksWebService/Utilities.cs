using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace SimpleVWorksWebService
{
    class Utilities
    {

        public static void Post(string body, string url)
        {


            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            byte[] bytes;

            bytes = System.Text.Encoding.ASCII.GetBytes(body);

            request.ContentType = "application/json";

            request.PreAuthenticate = true;

            request.ContentLength = bytes.Length;
            request.Method = "POST";

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();


        }

        //--Parse the JSON request from the LIMS
        public static payload GetRequestBody(HttpListenerRequest request)
        {
            if (!request.HasEntityBody)
            {
                return null;
            }
            using (System.IO.Stream body = request.InputStream) // here we have data
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(body, request.ContentEncoding))
                {
                   // return reader.ReadToEnd();

                    return JsonConvert.DeserializeObject<payload>(reader.ReadToEnd());
                  
                }
            }
        }

    }

    public class payload
    {
        public string action { get; set; }
        public string path { get; set; }
        public int numTimes { get; set; }
        public string run_id { get; set; }
        public object processVariables { get; set; }


    }
}
