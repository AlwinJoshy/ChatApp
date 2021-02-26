using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using ChatApp.Classes;

namespace ChatApp
{
    public class testClass
    {
        public string name;
        public int count;
    }

    class Program
    {

        public static byte[] ObjectToByte(object obj) 
        {
            BinaryFormatter bFormatter = new BinaryFormatter();

            using (MemoryStream ms = new MemoryStream()) {

                bFormatter.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static object ByteToObject(byte[] byteArray)
        {
            BinaryFormatter bFormatter = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(byteArray, 0, byteArray.Length);
                return bFormatter.Deserialize(ms);
            }
        }

        static void Main(string[] args)
        {
            
            if (args.Length > 0)
            {

                switch (args[0])
                {

                    case "-server":

                        int portNumber = -1;

                        if (args.Length >= 2) {
                            bool isPortValid = int.TryParse(args[1], out portNumber);
                            if (!isPortValid) throw new Exception("Port number is not valid");
                        }

                        else
                        {
                            throw new Exception("the arguments are not sufficent");
                        }

                        Server ChatServer = new Server(4432);
                        ChatServer.Run();
                        break;

                    case "-client":

                        IPAddress ipAddress;
                        int portClientNumber = -1;

                        if (args.Length >= 3)
                        {

                            bool ipAddressValid = IPAddress.TryParse(args[1], out ipAddress);
                            bool isPortValid = int.TryParse(args[2], out portClientNumber);

                            if (!isPortValid) throw new Exception("Port number is not valid");

                            if (!ipAddressValid) throw new Exception("IP address is not valid");
                        }

                        else {
                            throw new Exception("the arguments are not sufficent");
                        }

                        Client ChatClient = new Client(ipAddress, portClientNumber);
                        ChatClient.Run();
                 
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
