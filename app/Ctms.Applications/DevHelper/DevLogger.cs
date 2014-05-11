using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ctms.Applications.Common;
using Newtonsoft.Json;
using System.IO;

namespace Ctms.Applications.DevHelper
{
    public static class DevLogger
    {
        public static bool WriteFile = false;
        public static bool WriteOuput = true;

        public static void Log(string message)
        {
            if (WriteOuput == true) Console.WriteLine(message);
            if (WriteFile == true)  Configurator.Log.Debug(message);
        }

        public static void StoreObjectToJson(object obj, string fileName)
        {
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            File.WriteAllText("../../../Ctms.Applications/DevHelper/" + fileName + ".json", json);
        }

        public static T ParseJsonToObject<T>(string fileName)
        {
            string json = File.ReadAllText("../../../Ctms.Applications/DevHelper/" + fileName + ".json");
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
