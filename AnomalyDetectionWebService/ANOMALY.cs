//{anomalies: { col_name_1:[span_1], col_name_2:[span_1, span_2, ... ] ….},
//reason: Any} }

using System;
using System.Collections.Generic;

using Span = System.Collections.Generic.List<long>;

namespace AnomalyDetectionWebService
{
    // {“altitude_gps”: [100, 110, 20, 120…], “heading_gps”: [0.6, 0.59, 0.54, 0.51, ...] }
    public class ANOMALY
    {
        public Dictionary<string, List<Span>> anomalies { get; set; }
        public Dictionary<string, string> reason { get; set; }
    }
}


