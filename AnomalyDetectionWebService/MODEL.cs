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

        public static readonly string Status_Pending = "pending";
        public static readonly string Status_Ready = "ready";
        public static readonly string Status_Corrupted = "corrupted";
        public string CSVFileName()
        {
            return Program.NormalModelCSVFolder + model_id + ".csv";
        }
    }
}
