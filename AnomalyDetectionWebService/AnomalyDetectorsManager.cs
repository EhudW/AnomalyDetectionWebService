using System;
using System.Collections.Generic;
using System.Reflection;
using DLL;
using System.Threading.Tasks;

namespace AnomalyDetectionWebService
{
    
    public class NormalModelInfo
    {
        public static readonly string Status_NotFound = "NotFound";
        public static readonly string Status_Pending = "pending";
        public static readonly string Status_Ready = "ready";
        public IAnomalyDetector Detector = null;
        public MODEL jsonMODEL = null;
    }
    // manager class for IAnomalyDetector
    // syncronic class
    public class AnomalyDetectorsManager
    {
        public static readonly string pluginFolder = "plugins" + System.IO.Path.DirectorySeparatorChar;
        public static readonly string SimpleDetectorPath = pluginFolder + "SimpleAnomalyDetector.dll";
        public static readonly string HybdridDetectorPath = pluginFolder + "HybdridAnomalyDetector.dll";

        private readonly Dictionary<string, string> DllPath;
        private  Dictionary<int, NormalModelInfo> NormalModels;

        public AnomalyDetectorsManager() {
            this.DllPath = new Dictionary<string, string>()
            {
                { "regression", SimpleDetectorPath },
                { "hybrid", HybdridDetectorPath}
            };
            this.NormalModels = new Dictionary<int, NormalModelInfo>();
        }

        // example : ("hybrid", "flight_AFG45WQ.csv") -> true
        // where AFG45WQ should be uniqe
        // return unique ID for the model
        public MODEL AddNewDetector(string detectoionType)
        {
            if (!DllPath.ContainsKey(detectoionType)) return null;
            var detector = LoadAnomalyDetectorFromDLL(DllPath[detectoionType]);
            if (detector == null) return null;

            int id = 3456;
            while (NormalModels.ContainsKey(id))
                id = new Random().Next();

            MODEL json_model = new MODEL() { model_id = id, status = NormalModelInfo.Status_Pending, upload_time = DateTime.Now };
            NormalModels.TryAdd(id, new NormalModelInfo() { Detector = detector, jsonMODEL = json_model });
            return json_model;
        }
        //
        // example of use:
        //  AnomalyDetectorManager adm = new ...
        //  int id = adm.AddNewDetector("regression");
        //  if (id < 0) send error message to client
        //  else        send to client message with status = pending
        //  rename the csv to    "normalmodel" + id + ".csv"
        //  await? adm.Learn(id, "normalmodel" + id + ".csv")
        //  
        //
        public async Task<bool> Learn(int id, string normalCSV)
        {
            // check default features!
            if (!NormalModels.ContainsKey(id)) return false;
            return await Task.Run(()=>NormalModels[id].Detector.Learn(normalCSV));
        }

        // return IAnomalyDetector after testing or null if failed [id not exist or other bug]
        // useful IAnomalyDetector members :
        // NormalModel; MostCorrelativeWith; LastReports; LastDetection;
        public async Task<IAnomalyDetector> Detect(int id, string testCSV)
        {
            // check default features! + check there are at least prev features
            if (!NormalModels.ContainsKey(id)) return null;
            var detector = NormalModels[id].Detector.CloneDueNormalModel();
            bool isSuccess = await Task.Run(() => detector.Detect(testCSV));
            if (!isSuccess) return null;
            return detector;
        }

        public string getIdStatus(int id)
        {
            if (!NormalModels.ContainsKey(id)) return NormalModelInfo.Status_NotFound;
            return NormalModels[id].jsonMODEL.status;
        }
        // try to create new instance of detector,
        // according the given dll,and return it, or null if failed
        public static IAnomalyDetector LoadAnomalyDetectorFromDLL(string fromDllPath)
        {
            try
            {
                // dynamic link
                var dll = Assembly.LoadFile(fromDllPath);
                // get the type of the detector
                var type = dll.GetType("DLL.AnomalyDetector");
                // create new instance (dynamic, because it's .net library it is also "object" type for sure)
                dynamic instance = Activator.CreateInstance(type);
                // try to cast it to IAnomalyDetector
                return (IAnomalyDetector)instance;
            }
            catch
            {
            }
            return null;
        }
    }
}