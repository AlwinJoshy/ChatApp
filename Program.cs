using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace ChatApp
{
    class Program
    {
        static void Main(string[] args)
        {

            if (args.Length > 0)
            {

                switch (args[0])
                {

                    case "-server":
                        
                        Socket listeningSocket = new Socket(
                        AddressFamily.InterNetwork,
                        SocketType.Stream, // use Dragram to use UDP
                        ProtocolType.Tcp);

                        listeningSocket.Bind(new IPEndPoint(IPAddress.Any, 4432));
                        Console.WriteLine("Waiting for connections...");
                        listeningSocket.Listen(5);

                        Socket clientSocket = listeningSocket.Accept(); // new connection socket

                        clientSocket.Send(ASCIIEncoding.ASCII.GetBytes("Hello Client..."));

                        Byte[] reciveBuffer = new byte[1024];
                        clientSocket.Receive(reciveBuffer);

                        Console.WriteLine(ASCIIEncoding.ASCII.GetString(reciveBuffer));

                        break;

                    case "-client":

                        Socket socket;
                        socket = new Socket(
                            AddressFamily.InterNetwork,
                            SocketType.Stream,
                            ProtocolType.Tcp);

                        Console.WriteLine("Connecting to server...");
                        socket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4432));
                        Console.WriteLine("Connected to server...");

                        Byte[] reciveBufferBytes = new byte[1024];
                        socket.Receive(reciveBufferBytes);

                        Console.WriteLine(ASCIIEncoding.ASCII.GetString(reciveBufferBytes));

                        socket.Send(ASCIIEncoding.ASCII.GetBytes("Hello Server..."));

                        break;

                    default:
                        Console.WriteLine("Please provide arguments...");
                        break;

                }

            }
            else {
                Console.WriteLine("Please provide valid arguments...");
            }
        }
    }
}
