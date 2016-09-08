using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSs
{
    public enum PacketReceivedType
    {
        GETINFO,
        GETSTATUS,
        UNKNOWN
    }
    static class PacketTypes
    {
        public static byte[] HeartBeat = Convert("heartbeat Cod4\\n");
        public static byte[] HeartBeat_Flat = Convert("heartbeat flatline\n");
        public static byte[] GetServers = Convert("getservers 6 full empty");

        //stock = getIpAuthorize 2068329691 185.34.104.231 "" 0
        public static byte[] IpAuthorizePacket()
        {
            return Convert("getIpAuthorize " + GenerateNonce() + " 185.34.104.231 \"\" 0");
        }
        public static byte[] Convert(string text)
        {
            byte[] bufferTemp = Encoding.ASCII.GetBytes(text);
            byte[] bufferSend = new byte[bufferTemp.Length + 4];


            for (int i = 0; i < 4; i++)
                bufferSend[i] = 0xFF;
            for (int i = 0; i < bufferTemp.Length; i++)
                bufferSend[i + 4] = bufferTemp[i];

            return bufferSend;

        }
        public static string GenerateNonce()
        {
            var random = new Random();
            string s = "";
            for (int i = 0; i < 10; i++)
                s += random.Next(10);
            Print.Info("Nonce Generated: " + s);
            return s;
        }
    }
    public class PacketReceivedEventArgs : EventArgs
    {
        public System.Net.IPEndPoint from { get; set; }
        public string message { get; set; }
        public PacketReceivedType type { get; set; }
        public System.Net.Sockets.UdpClient stream { get; set; }
    }
  
}

