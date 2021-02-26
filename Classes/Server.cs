using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace ChatApp.Classes
{
    public class Server
    {

        List<Socket> socketConnections = new List<Socket>();
        Byte[] reciveBuffer = new byte[1024];
        Socket listeningSocket;
        public Server(int portNumber) {

            listeningSocket = new Socket(
           AddressFamily.InterNetwork,
           SocketType.Stream, // use Dragram to use UDP
           ProtocolType.Tcp);

            listeningSocket.Blocking = false;// sets the socket unblocking

            listeningSocket.Bind(new IPEndPoint(IPAddress.Any, portNumber));
            listeningSocket.Listen(5);
            Console.WriteLine("Server Initialized...");
        }

        public void Run() {

            while (true)
            {
                try
                {
                    // new connection socket
                    socketConnections.Add(listeningSocket.Accept());
                    Console.WriteLine("Socket Connected...");
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode != SocketError.WouldBlock) Console.WriteLine(ex);
                }

                for (int i = 0; i < socketConnections.Count; i++)
                {
                    try
                    {
                        int recivedDataSize = socketConnections[i].Receive(reciveBuffer); // try to recive data from the client
                        Console.WriteLine($"Message Recived from client No. {i} and recived data size is {recivedDataSize}");
                        for (int j = 0; j < socketConnections.Count; j++)
                        {
                            if (i != j) socketConnections[j].Send(reciveBuffer, recivedDataSize, SocketFlags.None); // try to send the data to other client
                        }
                    }
                    catch (SocketException ex)
                    {
                        if (ex.SocketErrorCode == SocketError.ConnectionAborted ||
                            ex.SocketErrorCode == SocketError.ConnectionReset)
                        {
                            socketConnections[i].Close();
                            socketConnections.RemoveAt(i);
                        }

                        if (ex.SocketErrorCode != SocketError.WouldBlock)
                        {
                            if (ex.SocketErrorCode != SocketError.ConnectionAborted ||
                            ex.SocketErrorCode != SocketError.ConnectionReset) Console.WriteLine(ex);

                        }
                    }
                }
            }

        }
    }


}
