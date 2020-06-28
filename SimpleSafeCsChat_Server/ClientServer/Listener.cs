using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleSafeCsChat_Server
{
    class Listener
    {
        int m_port;
        ServerManager m_serverManager = null;
        TcpListener m_listener = null;
        public Listener(int port, ServerManager serverManager)
        {
            m_port = port;
            m_serverManager = serverManager;
        }

        ~Listener()
        {
            if (m_listener != null)
            {
                m_listener.Stop();
            }
        }

        public void StartListen()
        {
            m_listener = new TcpListener(IPAddress.Any, m_port);
            m_listener.Start();

            while (true)
            {
                TcpClient client = m_listener.AcceptTcpClient();
                Thread thread = new Thread(new ParameterizedThreadStart(ClientThread));
                thread.Start(client);
            }
        }

        void ClientThread(Object StateInfo)
        {
            Proceed((TcpClient)StateInfo);
        }

        public void Proceed(TcpClient client)
        {
            string request = "";
            byte[] buffer = new byte[1024];
            int count;
            while ((count = client.GetStream().Read(buffer, 0, buffer.Length)) > 0)
            {
                request += Encoding.ASCII.GetString(buffer, 0, count);
                if (request.IndexOf("\r\n\r\n") >= 0)
                {
                    break;
                }
            }
            //remove trailing "\r\n\r\n"
            request = request.Substring(0, request.Length - 4);
            if (TryMatchConnectionCheck(request))
            {
                ReturnMessage(client, "OK");
            }
            else 
            {
                ReturnMessage(client, m_serverManager.ProcessRequest(request));
            }


            client.Close();

        }

        static void ReturnMessage(TcpClient client, string message)
        {
            try
            {
                NetworkStream tcpStream = client.GetStream();
                byte[] sendBytes = Encoding.ASCII.GetBytes(message + "\r\n\r\n");
                tcpStream.Write(sendBytes, 0, sendBytes.Length);
            }
            catch (SocketException ex)
            {
                Debug.WriteLine("Exception: " + ex.ToString());
            }
        }

        static bool TryMatchConnectionCheck(string request)
        {
            if (request.Equals("TRY"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
