using System;
using System.Collections.Generic;

namespace AnomalyDetectionWebService
{
    // {“altitude_gps”: [100, 110, 20, 120…], “heading_gps”: [0.6, 0.59, 0.54, 0.51, ...] }


    public class Predict_Data 
    {
        // method to check one list included in other
        public Dictionary<String, List<float>> predict_data { get; set; }
    }
    public class Train_Data 
    {
        // method to check one list included in other
        public Dictionary<String, List<float>> train_data { get; set; }
    }
}

