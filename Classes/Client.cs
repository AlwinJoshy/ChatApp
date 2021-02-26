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
    public class Client
    {

            Byte[] reciveBufferBytes = new byte[1024];
            Byte[] tempByteArray;
            string userName;
            string sendMessage = "";
            ConsoleKeyInfo pressedKey;
            Socket socket;
        public Client(IPAddress ipAddress, int portNumber) {

            socket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

                Console.WriteLine("Enter Your Name...");
                userName = Console.ReadLine();

                Console.WriteLine("Connecting to server...");
            //socket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4432));
            socket.Connect(new IPEndPoint(ipAddress, portNumber));
        }

        void ClearLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth - 1));
            Console.SetCursorPosition(0, Console.CursorTop);
        }

        void WriteOnConsole(string consoleText, int cursorAt) {
            ClearLine();
            Console.Write(consoleText);
        }

        public void Run() {


            socket.Blocking = false;

            while (true)
            {
                try
                {
                    while (Console.KeyAvailable)
                    {
                        pressedKey = Console.ReadKey(true);

                        if (pressedKey.Key == ConsoleKey.Enter)
                        {
                            byte[] sendData = ASCIIEncoding.ASCII.GetBytes($"{userName} : {sendMessage}");
                            int sendByteSize = socket.Send(sendData, sendData.Length, SocketFlags.None);
                            ClearLine();
                            Console.WriteLine($"{userName} : {sendMessage}", 0);
                            sendMessage = "";
                            WriteOnConsole("", 0);
                        }

                        else if (pressedKey.Key == ConsoleKey.Backspace && sendMessage.Length > 0) {
                            sendMessage = sendMessage.Remove(sendMessage.Length - 1);
                            WriteOnConsole(sendMessage, sendMessage.Length);
                        }

                        else
                        {
                            sendMessage += pressedKey.KeyChar.ToString();
                            WriteOnConsole(sendMessage, sendMessage.Length);
                        }
                        
                    }


                    int recivedByteCount = socket.Receive(reciveBufferBytes);
                    tempByteArray = reciveBufferBytes.Take(recivedByteCount).ToArray();
                    WriteOnConsole(ASCIIEncoding.ASCII.GetString(tempByteArray), 0);
                    Console.WriteLine();
                    WriteOnConsole(sendMessage, 0);

                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode != SocketError.WouldBlock) Console.WriteLine(ex);
                }

            }
        }

    }
}
