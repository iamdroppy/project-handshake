using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Reflection;
using ConsoleApplication1.Packets;

namespace ConsoleApplication1
{
    class NewSocket
    {
        private TcpListener tcpListener;
        private Thread listenThread;
        public static TcpClient tcpClient;
        public static NetworkStream clientStream;
        public static string pktData;
        public static string[] split;
        public static string[] slash;

        public void Server()
        {
            this.tcpListener = new TcpListener(IPAddress.Any, 90);
            this.listenThread = new Thread(new ThreadStart(ListenForClients));
            this.listenThread.Start();

            Console.WriteLine("Server is listening on port 90.\n");
        }

        public void ListenForClients()
        {
            try
            {
                this.tcpListener.Start();

                while (true)
                {
                    TcpClient client = this.tcpListener.AcceptTcpClient();

                    Console.WriteLine("Open connection [" + client.Client.RemoteEndPoint.ToString().Split(':')[0] + "]\n");
                    Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                    clientThread.Start(client);
                }
            }
            catch
            {
                Console.WriteLine("SOME FUCKING PROGRAM IS USING PORT 90! RAWR!");
            }
        }

        public void HandleClientComm(object client)
        {
            tcpClient = (TcpClient)client;
            clientStream = tcpClient.GetStream();

            SendData(clientStream, "HELLO");

            byte[] message = new byte[4000];
            int bytesRead;

            while (true)
            {
                bytesRead = 0;

                try
                {
                    bytesRead = clientStream.Read(message, 0, 4000);
                }
                catch
                {
                    break;
                }

                if (bytesRead == 0)
                {
                    break;
                }

                ASCIIEncoding encoder = new ASCIIEncoding();
                string NewData = encoder.GetString(message, 0, bytesRead);
                pktData = NewData.Substring(3);
                split = NewSocket.pktData.Split(' ');
                slash = NewSocket.pktData.Split('/');

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("[" + tcpClient.Client.RemoteEndPoint.ToString().Split(':')[0] + "]");
                Console.Write(" --> " + NewData.Substring(3) + "\n");
                Console.ResetColor();

                /*if (pktData.Contains("STATUS"))
                {
                    Console.WriteLine(" --sssss");
                    SendData(clientStream, "OK");
                }*/

                try
                {
                    Invoke<PacketHandler>(NewData.Split(' ')[2]);

                    /*Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("[" + tcpClient.Client.RemoteEndPoint.ToString().Split(':')[0] + "]");
                    Console.Write(" --> " + NewData.Split(' ')[2] + "\n");
                    Console.ResetColor();*/

                }
                catch { }
            }

            Console.WriteLine("Close connection [" + tcpClient.Client.RemoteEndPoint.ToString().Split(':')[0] + "]");
            //tcpClient.Client.Close();
        }

        public void Invoke<T>(string methodName) where T : new()
        {
            T instance = new T();
            MethodInfo method = typeof(T).GetMethod(methodName);
            method.Invoke(instance, null);
        }

        public static void SendData(NetworkStream clientStream, string Data)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();

            byte[] buffer = encoder.GetBytes("#" + Data + (char)13 + "##");
            clientStream.Write(buffer, 0, buffer.Length);
            clientStream.Flush();

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("["+ tcpClient.Client.RemoteEndPoint.ToString().Split(':')[0] + "]");
            Console.Write(" <-- " + Data + "\n");
            Console.ResetColor();
        }
        public static void SendData(NetworkStream clientStream, serverMessage Data)
        {
            NewSocket.SendData(clientStream, Data.ToString());
        }
    }
}
