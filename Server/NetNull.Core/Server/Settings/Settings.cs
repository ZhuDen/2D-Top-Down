using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Server.Settings
{

    internal class Settings
    {
        private static Parameters data;
        public static Parameters Data => data ?? (data = new Parameters());
        private static Settings instance;
        public static Settings Instance => instance ?? (instance = new Settings());

        public void LoadSettings()
        {
            if (File.Exists("settings\\Net.config"))
            {
                try
                {
                    data = JsonSerializer.Deserialize<Parameters>(File.ReadAllText("settings\\Net.config"));
                    Server.Elements.Logger.Log.Debug("Settings file loaded.");
                }
                    catch (Exception ex)
                {
                    Server.Elements.Logger.Log.Debug($"Settngs load error: {ex}");
                }
            }
            else {

                try
                {
                    Data.PORT = 8888;
                    Data.MAX_CONCURRENT_BATCHES = 2;
                    Data.MAX_BATCH_SIZE = 10;

                    string buffer = JsonSerializer.Serialize<Parameters>(Data, new JsonSerializerOptions
                    {
                        WriteIndented = true,
                    });
                    File.WriteAllText("settings\\Net.config", buffer);
                    Server.Elements.Logger.Log.Debug("Settings file create");
                }
                    catch (Exception ex)
                {
                    Server.Elements.Logger.Log.Debug($"Settngs create error: {ex}");
                }
        }
        }
    }

    internal class Parameters{

        public int MAX_BATCH_SIZE { get; set; }  // Максимальный размер пачки пакетов
        public int MAX_CONCURRENT_BATCHES { get; set; }  // Максимальное количество одновременно обрабатываемых пачек пакетов
        public int PORT { get; set; }


    }
}
