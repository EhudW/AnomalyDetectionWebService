using System;

namespace AnomalyDetectionWebService
{
    // { model_id: <int>, upload_time: <datetime>, status: “ready” | “pending” }

    public class MODEL
    {
        public int model_id { get; set; }
        private DateTime time;
        public DateTime upload_time { get => time; set { time = value.AddTicks(-(value.Ticks % TimeSpan.TicksPerSecond)); } }

        public string status { get; set; }

        public string CSVFileName()
        {
            return "normalmodel_" + model_id + ".csv";
        }
    }
}
