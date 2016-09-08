using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace FSs
{
    public class FServer
    {
        private IPEndPoint masterserver = new IPEndPoint(IPAddress.Parse("185.34.104.231"), 20810);
        private IPEndPoint masterauthserver = new IPEndPoint(IPAddress.Parse("185.34.104.231"), 20800);
        private string nonce;
        private Socket socket;
        public UdpClient newsock;


        public string Tag { get; set; }
        public int Port { get; set; }
        public Variables Variables { get; set; }
        public List<PlayersOnline> PlayersOnline { get; set; }

        public delegate void PacketEventhandler(FServer sender, PacketReceivedEventArgs e);
        public event PacketEventhandler PacketReceived;

        bool iSClosing = false;


        public override string ToString()
        {
            string s = "<FServer Info>\n";
            foreach (var prop in this.GetType().GetProperties())
                if (prop.Name != "Variables")
                    s += prop.Name + ": " + prop.GetValue(this, null) + "\n";
            s += "Server Variables: \n";
            foreach (var p in Variables.GetType().GetProperties())
                s += "\t" + p.Name + ": " + p.GetValue(Variables, null) + "\n";
            return s + "\n\n";
        }

        // stock = infoResponse\challenge\xxx\protocol\6\hostname\1234\mapname\mp_backlot\sv_maxclients\24\gametype\war\pure\1\maxPing\600\kc\1\hw\2\mod\0\voice\0\pb\0
        public string GenerateInfoString()
        {
            return "infoResponse\n\\challenge\\" + Variables.challenge + "\\protocol\\" + Variables.protocol + "\\hostname\\" + Variables.hostname + "\\mapname\\" + Variables.mapname + "\\clients\\"+Variables.clients+"\\sv_maxclients\\" + Variables.sv_maxclients + "\\gametype\\" + Variables.gametype + "\\pure\\" + Variables.pure + "\\maxPing\\" + Variables.sv_maxping + "\\kc\\" + Variables.kc + "\\hw\\" + Variables.hw + "\\mod\\" + Variables.mod + "\\voice\\" + Variables.voice + "\\pb\\" + Variables.pb;
        }
        //stock = statusResponse\g_compassShowEnemies\0\g_gametype\war\gamename\Call of Duty 4\mapname\mp_backlot\protocol\6\shortversion\1.7\sv_allowAnonymous\0\sv_disableClientConsole\0\sv_floodprotect\4\sv_hostname\1234\sv_maxclients\24\sv_maxPing\600\sv_maxRate\25000\sv_minPing\0\sv_privateClients\0\sv_punkbuster\0\sv_pure\1\sv_voice\1\ui_maxclients\32\challenge\-1051696178\pswrd\0\mod\0
        public string GenerateClientStatusString()
        {
            string status =  "statusResponse\n\\g_compassShowEnemies\\" + Variables.g_compassShowEnemies + "\\g_gametype\\" + Variables.gametype + "\\gamename\\BattleField 1\\mapname\\" + Variables.mapname + "\\protocol\\" + Variables.protocol + "\\shortversion\\" + Variables.shortversion + "\\sv_allowAnonymous\\" + Variables.sv_allowAnonymous + "\\sv_disableClientConsole\\" + Variables.sv_disableClientConsole + "\\sv_floodprotect\\" + Variables.sv_floodprotect + "\\sv_hostname\\" + Variables.hostname + "\\sv_maxclients\\" + Variables.sv_maxclients + "\\sv_maxPing\\" + Variables.sv_maxping + "\\sv_maxRate\\" + Variables.sv_maxRate + "\\sv_minPing\\" + Variables.sv_minping + "\\sv_privateClients\\" + Variables.sv_privateClients + "\\sv_punkbuster\\" + Variables.pb + "\\sv_pure\\" + Variables.pure + "\\sv_voice\\" + Variables.voice + "\\ui_maxclients\\" + Variables.sv_maxclients + "\\pswrd\\" + Variables.pswrd + "\\mod\\" + Variables.mod;
            foreach (PlayersOnline p in PlayersOnline)
                status += "\n" + p.score + " " + p.ping + " \"" + p.name + "\"";
            return status;
        }
        public string GenerateServerStatusString()
        {
            return "statusResponse\n\\g_compassShowEnemies\\" + Variables.g_compassShowEnemies + "\\g_gametype\\" + Variables.gametype + "\\gamename\\Call of Duty 4\\mapname\\" + Variables.mapname + "\\protocol\\" + Variables.protocol + "\\shortversion\\" + Variables.shortversion + "\\sv_allowAnonymous\\" + Variables.sv_allowAnonymous + "\\sv_disableClientConsole\\" + Variables.sv_disableClientConsole + "\\sv_floodprotect\\" + Variables.sv_floodprotect + "\\sv_hostname\\" + Variables.hostname + "\\sv_maxclients\\" + Variables.sv_maxclients + "\\sv_maxPing\\" + Variables.sv_maxping + "\\sv_maxRate\\" + Variables.sv_maxRate + "\\sv_minPing\\" + Variables.sv_minping + "\\sv_privateClients\\" + Variables.sv_privateClients + "\\sv_punkbuster\\" + Variables.pb + "\\sv_pure\\" + Variables.pure + "\\sv_voice\\" + Variables.voice + "\\ui_maxclients\\" + Variables.sv_maxclients + "challenge\\" + nonce + "\\pswrd\\" + Variables.pswrd + "\\mod\\" + Variables.mod;
        }
        public void Start()
        {
            Init();
            Console.CancelKeyPress += Console_CancelKeyPress;
        }

        private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            try
            {
                iSClosing = true;
                newsock.Close();
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.Bind(new IPEndPoint(IPAddress.Any, Port));
                socket.Connect(masterserver);
                Send(PacketTypes.HeartBeat_Flat);
                Print.Success("Closing Server with flatline");
                Print.Info("\nPress any key to exit");
                Console.ReadKey();
                Environment.Exit(0);
            }
            catch (Exception x) { Print.Error(x.Message); Environment.Exit(0); }
        }
        private void ConnectToMasterServer()
        {
            Print.Info(Tag + ": Connecting to Master server @" + masterserver.Address + ":" + masterserver.Port);
            socket.Connect(masterserver);
            Print.Success(Tag + ": Connected to Master MasterServer@" + masterserver.Address + ":" + masterserver.Port);
        }
        private void ConnectToMasterAuthServer()
        {
            Print.Success(Tag + ": Connecting to  MasterAuthServer@" + masterauthserver.Address + ":" + masterauthserver.Port);
            socket.Connect(masterauthserver);
            Print.Success(Tag + ": Connected to  MasterAuthServer@" + masterauthserver.Address + ":" + masterauthserver.Port);
        }
        private void Init()
        {
            try
            {
                Print.Info(Tag + ": Initialzing Socket");
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.Bind(new IPEndPoint(IPAddress.Any, Port));
                Print.Success(Tag + ": Socket Bound to Port " + Port);
                ConnectToMasterServer();
                Send(PacketTypes.HeartBeat_Flat);
                Thread.Sleep(1000);
                Send(PacketTypes.HeartBeat);
            }
            catch (Exception e) { Print.Error(e.Message); }

        }
        public void Send(byte[] p)
        {
            socket.Send(p);
            byte[] rec = new byte[2000];
            socket.BeginReceive(rec, 0, rec.Length, SocketFlags.None, new AsyncCallback(DataCallBack), rec);
        }
        public async void DataCallBack(IAsyncResult result)
        {

            byte[] receivedata = new byte[3000];
            receivedata = (byte[])result.AsyncState;

            string respon = Encoding.Default.GetString(receivedata).Replace('\0', ' ').Replace('ÿ', ' ').Trim();
            if(respon != "")
            Print.Success("Response: " + respon);
            //getstatus -1051696178
            //statusResponse\g_compassShowEnemies\0\g_gametype\war\gamename\Call of Duty 4\mapname\mp_backlot\protocol\6\shortversion\1.7\sv_allowAnonymous\0\sv_disableClientConsole\0\sv_floodprotect\4\sv_hostname\1234\sv_maxclients\24\sv_maxPing\600\sv_maxRate\25000\sv_minPing\0\sv_privateClients\0\sv_punkbuster\0\sv_pure\1\sv_voice\1\ui_maxclients\32\challenge\-1051696178\pswrd\0\mod\0

            if (respon.Contains("getstatus"))
            {
                nonce = respon.Replace("getstatus", "").Trim();
                Print.Success(Tag + ": Nonce detected @" + nonce);

                ConnectToMasterAuthServer();

                Print.Info(Tag + ": Sending Spoofed IpAuthorizePacket to MasterAuthServer@" + masterauthserver.Address + ":" + masterauthserver.Port);
                Send(PacketTypes.IpAuthorizePacket());

                ConnectToMasterServer();
                Print.Info(Tag + ": Sending Spoofed statusResponse to " + "MasterServer@" + masterserver.Address + ":" + masterserver.Port);
                Send(PacketTypes.Convert(GenerateServerStatusString()));

                Print.Info("\n\t\t\t\t"+Tag+" ^2Registered To MasterServer^7\n");
               await WaitForClient();
               return;
            }

            receivedata = new byte[3000];
            if (socket.Connected)
                socket.BeginReceive(receivedata, 0, receivedata.Length, SocketFlags.None, new AsyncCallback(DataCallBack), receivedata);
        }
        private Task WaitForClient()
        {
            socket.Close();
            return Task.Factory.StartNew(() =>{
            byte[] data = new byte[1024];
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 28960);
            newsock = new UdpClient(ipep);
            Print.Info("Waiting for a client...");
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            while (!iSClosing)
            {
                try
                {
                    data = newsock.Receive(ref sender);

                    string message = Encoding.ASCII.GetString(data, 0, data.Length);
                    PacketReceived(this,new PacketReceivedEventArgs{
                        from = sender,
                        message = Encoding.ASCII.GetString(data, 0, data.Length).Substring(4),
                        type = message.Contains("getstatus") ? PacketReceivedType.GETSTATUS : PacketReceivedType.GETINFO,
                        stream = newsock
                    });
                }
                catch (Exception e) { Print.Error(e.Message); }
            }
           });

        }
    }
    public class Variables
    {
        public string hostname { get; set; }
        public string mapname { get; set; }
        public string gametype { get; set; }
        public string shortversion { get; set; }
        public int sv_maxping { get; set; }
        public int sv_minping { get; set; }
        public int pb { get; set; }
        public int mod { get; set; }
        public int pswrd { get; set; }
        public int build { get; set; }
        public int pure { get; set; }
        public int sv_maxclients { get; set; }
        public int g_humanplayers { get; set; }
        public int g_compassShowEnemies { get; set; }
        public int sv_allowAnonymous { get; set; }
        public int sv_disableClientConsole { get; set; }
        public int sv_floodprotect { get; set; }
        public int sv_maxRate { get; set; }
        public int sv_privateClients { get; set; }
        public int clients { get; set; }
        public int type { get; set; }
        public int protocol { get; set; }
        public string challenge { get; set; }
        public int voice { get; set; }
        public int hw { get; set; }
        public int od { get; set; }
        public int hc { get; set; }
        public int kc { get; set; }
        public int ff { get; set; }
    }
    public class PlayersOnline
    {
        public string name { get; set; }
        public int ping { get; set; }
        public int score { get; set; }
    }

}
