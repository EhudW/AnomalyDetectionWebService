using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AnomalyAlgorithm
{
    public class IO_Util
    {
        public static bool SaveNormalModel(string outputName, List<CorrelatedFeatures> normal_model)
        {

            try
            {
                string jsonString = JsonSerializer.Serialize(normal_model);
                File.WriteAllText(outputName, jsonString);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static List<CorrelatedFeatures> LoadNormalModel(string sourceFile)
        {
            try
            {
                string jsonString = File.ReadAllText(sourceFile);
                return JsonSerializer.Deserialize<List<CorrelatedFeatures>>(jsonString);
            }
            catch
            {
                return null;
            }
        }
    }
}
