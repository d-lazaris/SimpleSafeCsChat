using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSafeCSChat
{
    static class ServerSpecificStrings
    {

        public static string MesageTypeOk = "Message=OK";

        public static string MesageTypeSend = "Message=Send";

        public static string MesageTypeGroupEnter = "Message=groupExit";

        public static string MesageTypeGroupExit = "Message=groupEnter";

        public static string MesageTypeUpdateStatus = "Message=UpdateStatus";

        public static string MesageTypeRegister = "Message=Register";

        public static string MesageTypeAuth = "Message=Auth";

        public static string MesageOk = "Message=OK";

        public static string MesageNotOnline = "Message=NotOnline";

        public static string MesageError = "Message=Error";

        public static string SendSessionKeyToUser(string selfName, string user, string key)
        {
            return String.Format("{0}&ChatType=Personal&sender={1}&target={2}&key={3}", ServerSpecificStrings.MesageTypeSend, selfName, user, key);
        }

        public static string SendMessageToUser(string selfName, string user, string text)
        {
            return String.Format("{0}&ChatType=Personal&sender={1}&target={2}&text={3}", ServerSpecificStrings.MesageTypeSend, selfName, user, text);
        }

        public static string SendMessageToGroup(string selfName, string chatName, string text)
        {
            return String.Format("{0}&ChatType=Group&sender={1}&target={2}&text={3}", ServerSpecificStrings.MesageTypeSend, selfName, chatName, text);
        }

        public static string SendMessageToEnterGroup(string selfName, string chatName, string password)
        {
            return String.Format("{0}&ChatType=Group&sender={1}&target={2}&password={3}", ServerSpecificStrings.MesageTypeGroupEnter, selfName, chatName, password);
        }

        public static string SendMessageToExitGroup(string selfName, string chatName)
        {
            return String.Format("{0}&ChatType=Group&sender={1}&target={2}", ServerSpecificStrings.MesageTypeGroupExit, selfName, chatName);
        }

        public static string AuthorizationRequest = "Message=OK";

        public static string RegistrationRequest = "Message=OK";

        public static string RecieveMessageToChat = "Message=OK";

        public static string ExtractParamValue(string paramName, string fullQs)
        {
            char[] delim = { '&' };
            string[] parameters = fullQs.Split(delim, StringSplitOptions.RemoveEmptyEntries);
            foreach (string parameter in parameters)
            {
                if (parameter.Contains(paramName) && parameter.Contains("="))
                {
                    char[] delimIn = { '=' };
                    return parameter.Split(delimIn, StringSplitOptions.RemoveEmptyEntries)[1];
                }
            }
            return "";
        }

        public static string PackedMessageDelim = "&pm=";
        public static string GetServerRsaPublicKey()
        {
            return @"<RSAKeyValue><Modulus>si2pEIKNPApfWeIkFMseRTeJm4UBGpDcqlUQvk9AIQTbFdKD5pNzXBRjogaHv98dnibD9JAZ8QNi5is9NGgt2agfmHWziNvmRLFrQwcICUohqULHmZstTYNsjTFy77hpGzPSkBJ3NTDoe1OKdOsCfB/Q/tdGfvIt+FZtPhnnX9WMRyeb7udOKW8R5AvEOM4qBEatLAb6ieaaU9wlMK+NA/SHTgycYsbtKy5/Zq6RKQgoODdc6UTKDnmbuEEssLvH2JM3W9MzGLOBLTn0XEOhs0iEpHfOg10h8NzTcmJeB1RP+KCdp8xB0Ddr2eep3JjZ41kgDlusIv2igqeRExbhQCqDyMA5GFKKkkPhf+KmQzWCFUDvGkL3Cd4oUP7ewwPP6cIb4DFdTE88g67koLHjeKT2TIyISe5LW1baQW8WKzPBTeOvARjsCC+AoZkA6YrSCK8nctwrDqTNzekzcQtuL0AP0a61S8n0mW6x/oJ/o5hLS8wMQuZ4l6s23Nm/T+ND+zu7JheaNrYHQIHJj/7F47Rx7rWhrdYUhg8So8JnFZ6RbO+Sg8ZY9JukqEKr6fZr1LDEJ0f3xVS2SIwcq3sCvVah5NjaE3ymHIY7n5gKgCy9gK8J2O6/vAEd0PheWMV/VnoGzyK8gCE/0GsSG2BkSGkpd/H8XhkkBio7vAJla8U=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        }
    }
}