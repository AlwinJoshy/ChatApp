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
        ConsoleColor userColor = Console.ForegroundColor;
        ConsoleKeyInfo pressedKey;
        Socket socket;


        public Client(IPAddress ipAddress, int portNumber)
        {

            socket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

            DisplayAndShowAskeColor();

            Console.WriteLine("Enter Your Name...");
            userName = Console.ReadLine();

            Console.WriteLine("Connecting to server...");
            //socket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4432));

            socket.Connect(new IPEndPoint(ipAddress, portNumber));
            Console.WriteLine("Connected");

            byte[] initialSend = Utility.ObjectToBytes(new Packet() { senderName = userName });

            socket.Send(initialSend, initialSend.Length, SocketFlags.None);

        }

        void DisplayAndShowAskeColor()
        {
            int colorID = 0;
            Object returnColorData = new object();

            // gets the property with the name from the anonymus class
            object GetProperty(object obj, string propertyName) => obj.GetType().GetProperty(propertyName).GetValue(obj);

            do
            {
                returnColorData = GetColorForID(colorID);
                if (returnColorData == null)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                }
                ConsoleColor consoleColor = (ConsoleColor)GetProperty(returnColorData, "consoleColor");
                Console.ForegroundColor = consoleColor;
                Console.WriteLine($"{colorID} = {GetProperty(returnColorData, "consoleColorName")}");
                colorID++;
            }
            while (returnColorData != null);


            Console.WriteLine("Please enter the color index of your liking and press enter.");

            bool revivedValidColor = false;
            int selectedColorIndex = -1;
            // 
            while (!revivedValidColor)
            {

                try
                {

                    bool indexValid = int.TryParse(Console.ReadLine(), out selectedColorIndex);
                    if (indexValid)
                    {
                        Object colorData = GetColorForID(selectedColorIndex);
                        if (colorData != null)
                        {
                            userColor = (ConsoleColor)GetProperty(colorData, "consoleColor");
                            revivedValidColor = true;
                        }
                        else
                        {

                            throw new Exception("the number should be amoung the given index.");
                        }
                    }
                    else
                    {
                        throw new Exception("please enter a valid number.");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

            }

        }

        Object GetColorForID(int colorID)
        {

            switch (colorID)
            {
                case 0:
                    return new { consoleColorName = "Red Rose", consoleColor = ConsoleColor.Red };
                case 1:
                    return new { consoleColorName = "Curry", consoleColor = ConsoleColor.Yellow };
                case 2:
                    return new { consoleColorName = "Azure", consoleColor = ConsoleColor.Blue };
                default:
                    return null;
            }

        }

        void ClearLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth - 1));
            Console.SetCursorPosition(0, Console.CursorTop);
        }

        void WriteOnConsole(string consoleText, int cursorAt)
        {
            ClearLine();
            Console.Write(consoleText);
        }

        public void Run()
        {


            socket.Blocking = false;

            while (true)
            {
                try
                {
                    while (Console.KeyAvailable)
                    {
                        pressedKey = Console.ReadKey(true);

                        // sending message
                        if (pressedKey.Key == ConsoleKey.Enter)
                        {
                            // created a packet and passed in all the data


                            byte[] sendData = Utility.ObjectToBytes(new Packet()
                            {
                                senderName = userName,
                                senderMessage = sendMessage,
                                senderColor = userColor
                            }
                            );

                            int sendByteSize = socket.Send(sendData, sendData.Length, SocketFlags.None);
                            ClearLine();
                            Console.ForegroundColor = userColor;
                            Console.WriteLine($"{userName} : {sendMessage}", 0);
                            Console.ResetColor();
                            sendMessage = "";
                            WriteOnConsole("", 0);
                        }
                        // back space and clearing
                        else if (pressedKey.Key == ConsoleKey.Backspace && sendMessage.Length > 0)
                        {
                            sendMessage = sendMessage.Remove(sendMessage.Length - 1);
                            WriteOnConsole(sendMessage, sendMessage.Length);
                        }

                        // display each chat the player presses
                        else
                        {
                            sendMessage += pressedKey.KeyChar.ToString();
                            WriteOnConsole(sendMessage, sendMessage.Length);
                        }

                    }

                    // recive the data from other clients
                    int recivedByteCount = socket.Receive(reciveBufferBytes);
                    tempByteArray = reciveBufferBytes.Take(recivedByteCount).ToArray();
                    // convert the arry back to packet
                    // 
                    Packet recivedPacket = (Packet)Utility.BytesToObject(tempByteArray);

                    Console.ForegroundColor = recivedPacket.senderColor;
                    WriteOnConsole($"{ recivedPacket.senderName} : { recivedPacket.senderMessage}", 0);
                    Console.ResetColor();
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
