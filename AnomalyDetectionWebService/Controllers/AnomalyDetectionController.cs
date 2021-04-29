using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        

        [Route("model")]
        [HttpPost]
        public MODEL UploadModelData([FromBody] DATA train_data, [FromQuery(Name = "model_type")] string model_type)
        {
           
            var model = adm.AddNewDetector(model_type);
            //  if (model == null) send error message to client; return;
            //  some code to write train_data to csv file named "normalmodel" + id + ".csv" remember to flush stdout!

            // Task.Run(() => adm.Learn(model.model_id, "normalmodel" + model.model_id + ".csv"));

            //  else        send to client message with status = pending
            return new MODEL() { model_id = 0, status = NormalModelInfo.Status_Pending, upload_time = DateTime.Now };
        }

        [Route("model")]
        [HttpGet]
        public MODEL CheckModelStatus([FromQuery(Name = "model_id")] int model_id)
        {
            // if (new AnomalyDetectorsManager().getIdStatus(model_id) == NormalModelInfo.Status_NotFound)  404 not found ??
            return new MODEL() { model_id = 0, status = NormalModelInfo.Status_Ready, upload_time = DateTime.Now };
        }

        [Route("model")]
        [HttpDelete]
       // [ProducesErrorResponseType(new Type]
        public MODEL DeleteModel([FromQuery(Name = "model_id")] int model_id)
        {
            if (adm.getIdStatus(model_id) == NormalModelInfo.Status_NotFound)
            {
                HttpContext.Response.StatusCode = 404;
                return null;
            }
            // if (new AnomalyDetectorsManager().getIdStatus(model_id) == NormalModelInfo.Status_NotFound)  404 not found ??
            return new MODEL() { model_id = 0, status = NormalModelInfo.Status_Ready, upload_time = DateTime.Now };
        }
        //HttpContext.Abort();
          //  ProducesErrorResponseTypeAttribute

    }
}
