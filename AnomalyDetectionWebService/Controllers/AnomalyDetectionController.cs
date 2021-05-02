using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

/* 
    http status codes from wikipedia: https://en.wikipedia.org/wiki/List_of_HTTP_status_codes
    200 OK
    201 Created
    202 Accepted
    204 No Content

    302 Found (Previously "Moved temporarily")

    400 Bad Request
    404 Not Found

    500 Internal Server Error
    503 Service Unavailable
*/
namespace AnomalyDetectionWebService.Controllers
{
    public class Counter
    {
        public int Count { get; set; }
        public int Decrease() { lock(this) { return --Count; } }
        public int Increase() { lock(this) { return ++Count; } }
    }

    [ApiController]
    [Route("api")]

    // L_var should be access only within "lock (L_var){}" and for SHORT time lock!
    public class AnomalyDetectionController : ControllerBase
    {
        // has to be static to save state between http request
        private static AnomalyDetectorsManager adm = new AnomalyDetectorsManager();
        private static Counter L_LearnDetectAmount = new Counter() { Count = 0 };
        private static readonly int MaxLearnDetectAmount = 20;


        /*
         * Request to server :   Post    /api/model?model_type=hybrid     Body=Train_Data(json)
         * Server does:          Learn new normal model(in background), and add to models list
         * 
         * Server responses:     [202 Accepted]                MODEL(json) -state of the new added model, before finishing the learning
         * Server responses:     [400 Bad Request]             unsupported model_type
         * Server responses:     [500 Internal Server Error]   on error with the addition
         * Server responses:     [503 Service Unavailable]     when unable to handle more than 20 learn & detect request at same time
         */
        [Route("model")]
        [HttpPost]
        public MODEL UploadModelData([FromBody] Train_Data data, [FromQuery(Name = "model_type")] string model_type)
        {
            if (!AnomalyAlgorithm.AnomalyDetection.IsSupportedMethod(model_type)) {
                HttpContext.Response.StatusCode = 400;
                return null;
            }
            int amount;
            lock(L_LearnDetectAmount) {L_LearnDetectAmount.Count++; amount = L_LearnDetectAmount.Count; }
            if (amount > MaxLearnDetectAmount)
            {
                lock(L_LearnDetectAmount) {L_LearnDetectAmount.Count--;}
                HttpContext.Response.StatusCode = 503;
                return null;
            }

            var model = adm.LearnAndAddNewModel(model_type, data, ()=> L_LearnDetectAmount.Decrease());
            if (model == null)
            {
                HttpContext.Response.StatusCode = 500;
                return null;
            }
            HttpContext.Response.StatusCode = 202;
            return model;
        }

        /*
         * Request to server :   Get    /api/model?model_id=0000
         * 
         * Server responses:     [200 OK]           MODEL(json) - state of the model
         * Server responses:     [404 Not Found]    when no model with the id was found
         */
        [Route("model")]
        [HttpGet]
        public MODEL CheckModelStatus([FromQuery(Name = "model_id")] int model_id)
        {
            var status = adm.getIdModel(model_id);
            if (status == null)
            {
                HttpContext.Response.StatusCode = 404;
                return null;
            }
            HttpContext.Response.StatusCode = 200;
            return status;
        }


        /*
        * Request to server :   DELETE    /api/model?model_id=0000
        * 
        * Server responses:     [204 No Content]     the model was deleted
        * Server responses:     [404 Not Found]      when no model with the id was found
        */
        [Route("model")]
        [HttpDelete]
        public void DeleteModel([FromQuery(Name = "model_id")] int model_id)
        {
            if (!adm.Remove(model_id))
            {
                HttpContext.Response.StatusCode = 404;
                return;
            }
            HttpContext.Response.StatusCode = 204;
        }

        /*
         * Request to server :   Get    /api/models
         * 
         * Server responses:     [200 OK]     List<MODEL>(json) - state of all models in the server
         */
        [Route("models")]
        [HttpGet]
        public List<MODEL> GetAviableNormalModels()
        {
            HttpContext.Response.StatusCode = 200;
            return adm.GetNormalModels();
        }


        /*
         * Request to server :   Post    /api/anomaly?model_id=0000     Body=Predict_Data(json)
         * Server does:          Load normal model(all in foreground) and calculate the span of the anomalies report
         * 
         * Server responses:     [200 OK]                                        ANOMLAY(json) 2 Dictionary of [feature:List<Span>] and [featureWhichHasReport:short string description]
         * Server responses:     [302 Found (Previously "Moved temporarily")]    if the model_id isn't ready (or exist), redirect to check it's state at [GET] /api/model?model_id={model_id}
         * Server responses:     [500 Internal Server Error]                     on error with the detection
         * Server responses:     [503 Service Unavailable]                       when unable to handle more than 20 learn & detect request at same time
         */
        [Route("anomaly")]
        [HttpPost]
        public ANOMALY DetectAnomalies([FromBody] Predict_Data data, [FromQuery(Name = "model_id")] int model_id)
        {
            if (!adm.IsReady(model_id)) { 
                HttpContext.Response.Redirect($"/api/model?model_id={model_id}", false); // false for 302 "Moved temporarily" rather than 301  "Moved Permanently"
                return null;
            }

            int amount;
            lock (L_LearnDetectAmount) { L_LearnDetectAmount.Count++; amount = L_LearnDetectAmount.Count; }
            if (amount > MaxLearnDetectAmount)
            {
                lock (L_LearnDetectAmount) { L_LearnDetectAmount.Count--; }
                HttpContext.Response.StatusCode = 503;
                return null;
            }

            var detection = adm.Detect(model_id, data);
            lock (L_LearnDetectAmount) { L_LearnDetectAmount.Count--; }
            if (detection == null)
            {
                    HttpContext.Response.StatusCode = 500;
                    return null;
             }
            HttpContext.Response.StatusCode = 200;
            return detection;
        }
    }
}
