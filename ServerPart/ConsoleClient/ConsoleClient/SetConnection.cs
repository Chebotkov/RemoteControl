using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace App1
{
    public static class SetConnection
    {
        public readonly static int port = 10000;
        
        public static void Set(byte[] message)
        {
            // Создаем локальную конечную точку

            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);
            // Создаем основной сокет
            try
            {
                Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Соединяем сокет с удаленной точкой
                sender.Connect(ipEndPoint);
                sender.Send(message);

                // Освобождаем сокет
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void SendMessage(string message, Socket sender)
        {
            byte[] msg = Encoding.UTF8.GetBytes(message);
            sender.Send(msg);
        }

    }
}
