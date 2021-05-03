using System;
using System.Collections.Generic;

namespace AnomalyDetectionWebService
{


    // { "predict_data" :{“altitude_gps”: [[100, 110],[200,210]…], “heading_gps”: [], ... } }
    public class Predict_Data 
    {
        // method to check one list included in other
        public Dictionary<String, List<float>> predict_data { get; set; }
    }

    // { "train_data" :{“altitude_gps”: [[100, 110],[200,210]…], “heading_gps”: [], ... } }
    public class Train_Data 
    {
        // method to check one list included in other
        public Dictionary<String, List<float>> train_data { get; set; }
    }
}

