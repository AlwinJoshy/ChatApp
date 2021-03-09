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
using Mono.Nat;


namespace ChatApp.Classes
{
    public class UPnPPunchthrough
    {

        public int privatePortID = 4432;
        public int publicPortID = 4432;

 
        void PunchthroughStart() {

           // NatUtility.Verbose = true;
           // NatUtility.Logger = Console.Out;

            NatUtility.DeviceFound += ADeviceFound;
            NatUtility.DeviceLost += ADeviceLost;


            NatUtility.StartDiscovery();
  /*
            while (true)
            {
                Thread.Sleep(500000);
                NatUtility.StopDiscovery();
                NatUtility.StartDiscovery();
            }
            */
        }

        public UPnPPunchthrough()
        {
            PunchthroughStart();
            Console.WriteLine("UPnP punchthrough initiated...");
        }

        public UPnPPunchthrough(int privatePortID, int publicPortID)
        {
            this.privatePortID = privatePortID;
            this.publicPortID = publicPortID;

            Console.WriteLine("UPnP punchthrough initiated...");
            PunchthroughStart();

        }

        public void StopDiscovery()
        {
            NatUtility.StopDiscovery();
        
        }

        public void ADeviceFound(object sender, DeviceEventArgs args)
        {
            Console.WriteLine("Device Found...");
            INatDevice device = args.Device;
            device.CreatePortMap(new Mapping(Protocol.Tcp, privatePortID, publicPortID));
        }

        public void ADeviceLost(object sender, DeviceEventArgs args)
        {
            Console.WriteLine("Device Disconected...");
            INatDevice device = args.Device;
            device.DeletePortMap(new Mapping(Protocol.Tcp, privatePortID, publicPortID));
        }

        public void UnhandledExceptionEventArgs(object exception, bool isTerminating) { 

        }

    }
}
