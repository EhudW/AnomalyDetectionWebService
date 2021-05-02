using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using AnomalyAlgorithm;

namespace AnomalyDetectionWebService
{
    public class ExtendedModelInfo
    {
        public MODEL info { get; set; }
        public List<CorrelatedFeatures> normal_model { get; set; }
    }
    public class IO_Util
    {
        public static bool SaveNormalModel(string outputName, List<CorrelatedFeatures> normal_model, MODEL description)
        {
            ExtendedModelInfo info = new ExtendedModelInfo() { info = description, normal_model = normal_model };
            try
            {
                string jsonString = JsonSerializer.Serialize(info);
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
            return RestoreExtendedModelInfo(sourceFile)?.normal_model;
        }
        public static ExtendedModelInfo RestoreExtendedModelInfo(string sourceFile)
        {
            try
            {
                string jsonString = File.ReadAllText(sourceFile);
                return JsonSerializer.Deserialize<ExtendedModelInfo>(jsonString);
            }
            catch
            {
                return null;
            }
        }
    }
}
