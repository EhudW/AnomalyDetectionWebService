using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

// todo
// delete .tmp files
// check the system works :)
// maybe add store cf [normal model in files rather than the ram {both dll should be changed: cpp dll + c# dll}
namespace AnomalyDetectionWebService.Controllers
{
    [ApiController]
    [Route("api")]
    public class AnomalyDetectionController : ControllerBase
    {

        // private readonly ILogger<AnomalyDetectionController> _logger;

        public AnomalyDetectionController()//ILogger<AnomalyDetectionController> logger)
        {
            // _logger = logger;
        }
        private AnomalyDetectorsManager adm = new AnomalyDetectorsManager();


        private static bool writeToCsv(string csvName, Dictionary<String, List<float>> data)
        {
            try
            {
                string[] features = data.Keys.ToArray();
                int minSize = 0;
                foreach (string f in features) minSize = Math.Min(minSize, data[f].Count);

                using (FileStream outFile = System.IO.File.Open(csvName, FileMode.Create))
                {

                    var output = new StreamWriter(outFile);

                    String featuresLine = String.Join(',', features);
                    output.Write(featuresLine + "\n");
                    output.Flush();

                    for (int i = 0; i < minSize; i++)
                    {
                        bool notFirst = false;
                        foreach (string f in features)
                        {
                            if (notFirst) output.Write(',');
                            output.Write(data[f][i]);
                            notFirst = true;
                        }
                        output.Write('\n');
                        output.Flush();
                    }

                }
                return true;
            } catch { return false; }
        }

        [Route("model")]
        [HttpPost]
        public MODEL UploadModelData([FromBody] Train_Data data, [FromQuery(Name = "model_type")] string model_type)
        {
            var model = adm.AddNewDetector(model_type);
            if (model == null || !writeToCsv(model.CSVFileName(), data.train_data))
            {
                HttpContext.Response.StatusCode = 500;
                return null;
            }
            Task.Run(() => adm.Learn(model.model_id, model.CSVFileName()));
            return new MODEL() { model_id = 0, status = NormalModelInfo.Status_Pending, upload_time = DateTime.Now };
        }

        [Route("model")]
        [HttpGet]
        public MODEL CheckModelStatus([FromQuery(Name = "model_id")] int model_id)
        {
            var status = adm.getIdStatus(model_id);
            if (status == null)
            {
                HttpContext.Response.StatusCode = 404;
                return null;
            }
            return status;
        }

        [Route("model")]
        [HttpDelete]
        public void DeleteModel([FromQuery(Name = "model_id")] int model_id)
        {
            if (!adm.Remove(model_id))
            {
                HttpContext.Response.StatusCode = 404;
                return;
            }
        }

        [Route("models")]
        [HttpGet]
        public List<MODEL> GetAviableNormalModels()
        {
            return adm.GetAviableNormalModels();
        }


        private static string getRandomFileName(int modelID)
        {
            int num = 3456;
            while (System.IO.File.Exists($"model_{modelID}_test_{num}.csv"))
                num = new Random().Next();
            return $"model_{modelID}_test_{num}.csv";
        }
        [Route("anomaly")]
        [HttpPost]
        public async Task<ANOMALY> DetectAnomalies([FromBody] Predict_Data data, [FromQuery(Name = "model_id")] int model_id)
        {
            if (!adm.IsReady(model_id)) { 
                HttpContext.Response.Redirect($"/api/model?model_id={model_id}", false);
                return null;
            }
            string fileName = getRandomFileName(model_id);
            bool isSuccess = writeToCsv(fileName, data.predict_data);
            var detector = isSuccess ? await adm.Detect(model_id, fileName) : null;
            if (detector == null)
            {
                    HttpContext.Response.StatusCode = 500;
                    return null;
             }

            var spanDictionary = new Dictionary<string, List<Span>>();
            foreach (string f in data.predict_data.Keys)
            {
                var f_span = new List<Span>();
                long range_start = -100;
                long last_timestep = -100;
                foreach (var report in detector.LastReports(f))
                {
                    if (range_start == -100) {
                        range_start = report.timeStep;
                        last_timestep = report.timeStep;
                        continue;
                    }
                    if (last_timestep + 1 == report.timeStep)
                    {
                        last_timestep = report.timeStep;
                        continue;
                    }
                    f_span.Add(new Span() { start = range_start, end = last_timestep });
                    last_timestep = report.timeStep;
                    range_start = report.timeStep;
                }
                if (range_start != -100)
                {
                    f_span.Add(new Span() { start = range_start, end = last_timestep });
                }
                spanDictionary.Add(f, f_span);
            }
            return new ANOMALY() { anomalies = spanDictionary , reason = "ANY"};// reason=any??
            // delete fileName file??
        }

        /* if (model_id == 1)
         {
             HttpContext.Response.Redirect("/", false);
             return null;
         }*/
    }
}
