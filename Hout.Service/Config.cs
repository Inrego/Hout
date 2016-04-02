using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hout.Models.Device;
using Newtonsoft.Json;

namespace Hout.Service
{
    public class Config
    {
        #region Save/Load
        public static string ConfigLocation { get; } =
            Path.Combine(Path.GetDirectoryName(typeof (Config).Assembly.Location),
                "config.json");

        private static Config _instance;
        public static Config Instance
        {
            get
            {
                if (_instance == null)
                {
                    try
                    {
                        _instance = JsonConvert.DeserializeObject<Config>(File.ReadAllText(ConfigLocation));
                    }
                    catch (Exception)
                    {
                        _instance = new Config();
                    }
                }
                return _instance;
            }
        }

        public static void Save()
        {
            File.WriteAllText(ConfigLocation, JsonConvert.SerializeObject(Instance, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            }));
        }
        #endregion Save/Load
        public List<BaseDevice> Devices { get; set; } 
    }
}
