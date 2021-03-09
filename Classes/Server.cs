using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Threading;
using Open.Nat;

namespace ChatApp.Classes
{

    public class Server
    {
        /// UPnP stuff
        int portNumber = -1111;
        int initialAttemptMaxCount = 100;
        public bool UPnpPunchThrough = false;

        List<Socket> socketConnections = new List<Socket>();
        List<string> userDetails = new List<string>();


        Byte[] reciveBuffer = new byte[1024];
        Socket listeningSocket;

        // open nat test
        public async Task GetExternalIP(int privetPort)
        {
            var discoverer = new NatDiscoverer();

            var device = await discoverer.DiscoverDeviceAsync();

            // display the NAT's IP address
            Console.WriteLine("The external IP Address is: {0} ", await device.GetExternalIPAsync());


            // setting external to 420
            await device.CreatePortMapAsync(new Mapping(Protocol.Tcp, privetPort, 5040, "For testing"));

            var cts = new CancellationTokenSource(8000);

            // we are only interested in Upnp NATs because PMP protocol doesn't allow to list mappings
            var newDevice = await discoverer.DiscoverDeviceAsync(PortMapper.Upnp, cts);

            foreach (var mapping in await newDevice.GetAllMappingsAsync())
            {
                Console.WriteLine(mapping);
              //  await device.DeletePortMapAsync(mapping);
            }



            UPnpPunchThrough = true;
        }

        public Server(int portNumber)
        {
            this.portNumber = portNumber;
            StartServer();
        }

        async Task StartServer() {

            //  UPnPPunchthrough monoUPnP = new UPnPPunchthrough(4432, 500);

            // open nat test
            await GetExternalIP(portNumber);

            listeningSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            listeningSocket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);

            listeningSocket.SetIPProtectionLevel(IPProtectionLevel.Unrestricted);

            listeningSocket.Blocking = false;// sets the socket unblocking

            listeningSocket.Bind(new IPEndPoint(IPAddress.Any, portNumber));
            listeningSocket.Listen(5);
            Console.WriteLine("Server Initialized...");
        }

        public void Run()
        {
            Console.WriteLine("Running...");
            while (true)
            {
                try
                {
                    // new connection socket
                    Socket newConnectionSocket = listeningSocket.Accept();
                    socketConnections.Add(newConnectionSocket);
                    Console.WriteLine("Socket Connected...");

                    int captureAttempts = initialAttemptMaxCount;

                    // try to get the first data
                    while (captureAttempts > 0)
                    {
                        try
                        {
                            newConnectionSocket.Receive(reciveBuffer);
                            string connectorName = ((Packet)Utility.BytesToObject(reciveBuffer)).senderName;
                            userDetails.Add(connectorName);

                            Console.WriteLine($"{connectorName} joined the server.");

                            byte[] sendData = Utility.ObjectToBytes(new Packet()
                            {
                                senderColor = ConsoleColor.Magenta,
                                senderMessage = $"{connectorName} Joined the Server !!!",
                                senderName = $"[ SERVER ] => "
                            }
                            );

                            // notify entry of a new user to server
                            SendDataToAll(socketConnections, socketConnections.Count - 1, sendData);

                            break;
                        }
                        catch (SocketException ex)
                        {
                            if (ex.SocketErrorCode != SocketError.WouldBlock) Console.WriteLine(ex);
                        }
                        captureAttempts--;
                    }
                    // unable to capture the first data
                    if (captureAttempts == 0)
                    {
                        Console.WriteLine($"Server Timed out after {initialAttemptMaxCount} data capture attempts");
                        socketConnections.RemoveAt(socketConnections.Count - 1);
                        newConnectionSocket.Close();
                    }

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
                        SendDataToAll(socketConnections, i, reciveBuffer);
                        Console.WriteLine($"Message Recived from client No. {i} and recived data size is {recivedDataSize}");
                    }
                    catch (SocketException ex)
                    {
                        if (ex.SocketErrorCode == SocketError.ConnectionAborted ||
                            ex.SocketErrorCode == SocketError.ConnectionReset)
                        {
                            socketConnections[i].Close();
                            socketConnections.RemoveAt(i);

                            byte[] sendData = Utility.ObjectToBytes(new Packet()
                            {
                                senderColor = ConsoleColor.Magenta,
                                senderMessage = $"{userDetails[i]} Left the Server !!!",
                                senderName = $"[ SERVER ] => "
                            }
                            ); ;

                            // notify diconnetion of the server
                            SendDataToAll(socketConnections, -1, sendData);

                            Console.WriteLine($"{userDetails[i]} Removed..");
                            userDetails.RemoveAt(i);
                        }

                        if (ex.SocketErrorCode != SocketError.WouldBlock)
                        {
                            if (ex.SocketErrorCode != SocketError.ConnectionAborted ||
                            ex.SocketErrorCode != SocketError.ConnectionReset)
                            {
                                if (ex.SocketErrorCode == SocketError.ConnectionReset)
                                {
                                    // connection was terminated by a client
                                    /*
                                    byte[] sendMesage = ASCIIEncoding.UTF8($"{userDetails[i]} : ")
                                    socketConnections[i]
                                    */
                                }
                                else
                                {
                                    Console.WriteLine(ex);
                                }



                            }

                        }
                    }
                }
            }

        }


        void SendDataToAll(List<Socket> socketList, int exclussionNumber, byte[] sendData)
        {
            for (int j = 0; j < socketList.Count; j++)
            {
                if (exclussionNumber != j) socketList[j].Send(sendData, sendData.Length, SocketFlags.None); // try to send the data to other client
            }
        }

    }
}
