//{anomalies: { col_name_1:[span_1], col_name_2:[span_1, span_2, ... ] ….},
//reason: Any} }

using System;
using System.Collections.Generic;

namespace AnomalyDetectionWebService
{
    // {“altitude_gps”: [100, 110, 20, 120…], “heading_gps”: [0.6, 0.59, 0.54, 0.51, ...] }

    public class Span
    {
        int start; //include  //notice timestep is start from 0
        int end; //exclude
    }
    public class ANOMALY
    {
        public Dictionary<string, Span> anomalies { get; set; }
        public string reason { get; set; }
    }
}


