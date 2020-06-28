using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSafeCSChat
{
    public class Networker
    {
        string m_address;
        int m_port;
        public Networker(string address, int port)
        {
            m_address = address;
            m_port = port;
        }

        public static bool TryConnect(string address, int port)
        {
            string response = SendMessageAndWaitResponse("TRY", address, port);
            if (response.Equals("OK"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string SendMessageAndWaitResponse(string message, string address, int port)
        {
            IPAddress ipAddr = IPAddress.Parse(address);
            IPEndPoint endPoint = new IPEndPoint(ipAddr, port);
            TcpClient newClient = new TcpClient();
            byte[] Buffer = new byte[1024];
            string response = "";
            int Count;
            try
            {
                newClient.Connect(endPoint);
                NetworkStream tcpStream = newClient.GetStream();
                byte[] sendBytes = Encoding.ASCII.GetBytes(message + "\r\n\r\n");
                tcpStream.Write(sendBytes, 0, sendBytes.Length);
                newClient.ReceiveTimeout = 2000;
                while ((Count = tcpStream.Read(Buffer, 0, Buffer.Length)) > 0)
                {
                    response += Encoding.ASCII.GetString(Buffer, 0, Count);
                    if (response.IndexOf("\r\n\r\n") >= 0)
                    {
                        break;
                    }
                }
                //remove trailing "\r\n\r\n"
                response = response.Substring(0, response.Length - 4);
                return response;
            }
            catch (SocketException ex)
            {
                Debug.WriteLine("Exception: " + ex.ToString());
                return "";
            }
        }

        public bool SendMessage(string message)
        {
            return true;
        }

        public string SendMessageAndWaitResponse(string message)
        {
            return SendMessageAndWaitResponse(message, m_address, m_port);
        }

        
    }
}
