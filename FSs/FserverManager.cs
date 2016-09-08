using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace FSs
{
    class FserverManager
    {
        public List<FServer> FServers { get; set; }
        private string jsonpath { get; set; }

        public FserverManager(string jsonpath)
        {
            if (File.Exists(jsonpath))
                Print.Success(jsonpath + " Found");
            else
                Print.Error(jsonpath + "Not found", true);

            this.jsonpath = jsonpath;
        }
        public void Init()
        {
            Print.Info("Parsing " + this.jsonpath + "..");
            try
            {
                FServers = JsonConvert.DeserializeObject<List<FServer>>(File.ReadAllText(this.jsonpath));
                string servers = "[";
                foreach (FServer s in FServers)
                    servers += "^2" + s.Tag + "^7:^2" + s.Port + "^7,";
                Print.Success(this.jsonpath + " loaded with ^2" + FServers.Count + " ^7entries " + servers.Substring(0, servers.Length - 1) + "]");
            }
            catch (Exception e)
            {
                Print.Error(e.Message);
            }
        }
    }
}
