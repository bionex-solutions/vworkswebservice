using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using IniParser;
using IniParser.Model;

namespace API_CONSTANTS
{
    public class VWORKS
    {

        public static readonly string USER_NAME = ConfigParser.ParseINI("VWORKS", "user_name");
        public static readonly string PASSWORD = ConfigParser.ParseINI("VWORKS", "password");
        public static readonly string WORKING_DIRECTORY = ConfigParser.ParseINI("VWORKS", "working_directory");

    }

    public class ENDPOINT_SETTINGS
    {
        public static readonly string URL = ConfigParser.ParseINI("ENDPOINT_SETTINGS", "url");
        public static readonly string ACCESS_CONTROL = ConfigParser.ParseINI("ENDPOINT_SETTINGS", "access_control");
    }

    public class WEB_SOCKET_SETTINGS
    {
        public static readonly string URL = ConfigParser.ParseINI("WEB_SOCKET_SETTINGS", "url");
        public static readonly int PORT = Int32.Parse( ConfigParser.ParseINI("WEB_SOCKET_SETTINGS", "port"));
    }

    // urls to lims endpoints that will handle VWorks Events
    public class EVENT_ENDPOINTS
    {
      
        public static readonly string INITIALIZATION_COMPLETE = ConfigParser.ParseINI("EVENT_ENDPOINTS", "init_complete");
        public static readonly string PROTOCOL_COMPLETE = ConfigParser.ParseINI("EVENT_ENDPOINTS", "protocol_complete");
        public static readonly string RECOVERABLE_ERROR = ConfigParser.ParseINI("EVENT_ENDPOINTS", "recoverable_error");
        public static readonly string UNRECOVERABLE_ERROR = ConfigParser.ParseINI("EVENT_ENDPOINTS", "unrecoverable_error");
        public static readonly string LOG_MESSAGE = ConfigParser.ParseINI("EVENT_ENDPOINTS", "log_message");
        public static readonly string MESSAGE_BOX_ACTION = ConfigParser.ParseINI("EVENT_ENDPOINTS", "message_box_action");
        public static readonly string PROTOCOL_ABORTED = ConfigParser.ParseINI("EVENT_ENDPOINTS", "protocol_aborted");
        public static readonly string USER_MESSAGE = ConfigParser.ParseINI("EVENT_ENDPOINTS", "user_message");

    }


    // urls to lims endpoints that will handle VWorks Events
    public class EVENT_ENDPOINT_SWITCH
    {

        public static readonly Boolean INITIALIZATION_COMPLETE = Convert.ToBoolean(ConfigParser.ParseINI("EVENT_ENDPOINT_SWITCH", "init_complete"));
        public static readonly Boolean PROTOCOL_COMPLETE = Convert.ToBoolean(ConfigParser.ParseINI("EVENT_ENDPOINT_SWITCH", "protocol_complete"));
        public static readonly Boolean CLOSE_PROTOCOL = Convert.ToBoolean(ConfigParser.ParseINI("EVENT_ENDPOINT_SWITCH", "close_protocol"));
        public static readonly Boolean RECOVERABLE_ERROR = Convert.ToBoolean(ConfigParser.ParseINI("EVENT_ENDPOINT_SWITCH", "recoverable_error"));
        public static readonly Boolean UNRECOVERABLE_ERROR = Convert.ToBoolean(ConfigParser.ParseINI("EVENT_ENDPOINT_SWITCH", "unrecoverable_error"));
        public static readonly Boolean LOG_MESSAGE = Convert.ToBoolean(ConfigParser.ParseINI("EVENT_ENDPOINT_SWITCH", "log_message"));
        public static readonly Boolean MESSAGE_BOX_ACTION = Convert.ToBoolean(ConfigParser.ParseINI("EVENT_ENDPOINT_SWITCH", "message_box_action"));
        public static readonly Boolean PROTOCOL_ABORTED = Convert.ToBoolean(ConfigParser.ParseINI("EVENT_ENDPOINT_SWITCH", "protocol_aborted"));
        public static readonly Boolean USER_MESSAGE = Convert.ToBoolean(ConfigParser.ParseINI("EVENT_ENDPOINT_SWITCH", "user_message"));

    }

    class ConfigParser
    {
  
        public static string ParseINI(string section, string key)
        {

            var parser = new FileIniDataParser();
            IniData config = parser.ReadFile(@"C:\sites\SimpleVWorksWebService\SimpleVWorksWebService\config.ini");

            return config[section][key];

        }

    }
}
