using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft;
using System.IO;
using System.Threading;

namespace FSs
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "";
            Print.Info("\r \n^7Title: ^3Fake Server Launcher^7\nAuthor: ^3Poyser^7\n\nHow to: ^3Make Changes to config.json to add/edit multiple servers^7 ^2:)\n^5CTRL+C ^7to to Unresgister servers and exit.\n");
            FserverManager fM = new FserverManager("config.json");
            fM.Init();

            foreach (FServer s in fM.FServers)
            {
                Print.Info("Starting " + s.Tag + "..");
                s.PacketReceived += PacketReceived;
                s.Start();
            }
            Thread.Sleep(Timeout.Infinite);
        }

        private static void PacketReceived(FServer sender, PacketReceivedEventArgs e)
        {
            string from = e.from.Address + ":" + e.from.Port;
            byte[] response;
            switch (e.type)
            {
                case PacketReceivedType.GETINFO:
                    Print.Request("GetInfo Request Packet Received From " + from);
                    response = PacketTypes.Convert(sender.GenerateInfoString());
                    e.stream.Send(response, response.Length, e.from);
                    Print.Response(sender.Tag + " info sent to " + from);
                    break;
                case PacketReceivedType.GETSTATUS:
                    Print.Request("GetStatus Request Packet Received From " + from);
                    response = PacketTypes.Convert(sender.GenerateClientStatusString());
                    e.stream.Send(response, response.Length, e.from);
                    Print.Response(sender.Tag + " status-info sent to " + from);
                    break;
                default:
                    break;
            }
        }
    }
}
