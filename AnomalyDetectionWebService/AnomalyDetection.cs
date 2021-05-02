/*
 *          string method = circle regression hybrid
            // var x = AnomalyAlgorithim.GetNormal(parse(learn), method);
               var x = IO_Util.LoadNormalModel(s3); 
            //IO_Util.SaveNormalModel(s3, x);
            var y = AnomalyAlgorithim.GetDetection(parse(detect), x);
            
            ###AnomalyAlgorithim.cs IO_Util.cs MathUtil.cs OnDiffTask
            ###public static Dictionary<string, List<Span>> ToSpanDictionary(Dictionary<string, List<int>> detection)
            ###public static Dictionary<string,string> GetReportTypes(List<CorrelatedFeatures> model,  Dictionary<string, List<int>> detection)
            

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json.Serialization;

// public static List<CorrelatedFeatures> GetNormal(Dictionary<string, List<float>> features, string method, bool commutative = true)
// public static Dictionary<string,List<int>> GetDetection(Dictionary<string, List<float>> features , List<CorrelatedFeatures> normal_model)
namespace AnomalyAlgorithm
{
    public class CorrelatedFeatures
    {
        // names of the correlated features
        public string feature1 { get; set; }
        public string feature2 { get; set; }
        public float correlation { get; set; }
        public float threshold { get; set; }
        public string typeName { get; set; }
        public Line lin_reg { get; set; }
        public Circle minimal_circ { get; set; }
        public CorrelatedFeatures(string feature1, string feature2, float correlation, float threshold, string typeName, Object detectionObject)
        {
            this.feature1 = feature1;
            this.feature2 = feature2;
            this.correlation = correlation;
            this.threshold = threshold;
            this.typeName = typeName;
            this.lin_reg = detectionObject as Line;
            this.minimal_circ = detectionObject as Circle;
        }
        [JsonConstructorAttribute]
        public CorrelatedFeatures() { }
    }
    public class Span
    {
        public long start { get; set; } //include  //notice timestep is start from 0
        public long end { get; set; }  //exclude
    }
    public class AnomalyDetection
    {
        private delegate bool AnomalousChecker(float x, float y, CorrelatedFeatures c);
        private static readonly Dictionary<string, AnomalousChecker> CheckerMethods = new Dictionary<string, AnomalousChecker>() {
            { "regression", AnomalousChecker_linear},
            { "circle", AnomalousChecker_circle},
            { "hybrid", AnomalousChecker_hybrid}

        };

        private delegate object[] ThresholdFactory(float[] x, float[] y); // <float, string, object>
        // must check each float value x is: AnomalyAlgorithim.IsRegularNum(x)
        private static readonly Dictionary<string, ThresholdFactory> ThresholdMethods = new Dictionary<string, ThresholdFactory>() {
            { "regression", ThresholdFactory_linear},
            { "circle", ThresholdFactory_circle},
            { "hybrid", ThresholdFactory_hybrid}

        };

        public static bool IsSupportedMethod(string algorithmMethod)
        {
            return CheckerMethods.ContainsKey(algorithmMethod);
        }
        public static Dictionary<string, CorrelatedFeatures> GetNormalModelDictionary(Dictionary<string, List<float>> features, string method)
        {
            var list = GetNormal(features, method);
            Dictionary<string, CorrelatedFeatures> result = new Dictionary<string, CorrelatedFeatures>();
            foreach (var x in list) result.TryAdd(x.feature1, x);
            return result;
        }
        public static List<CorrelatedFeatures> GetNormal(Dictionary<string, List<float>> features, string method, bool commutative = true)
        {
            if (!ThresholdMethods.ContainsKey(method)) return new List<CorrelatedFeatures>();
            return GetNormal(features, ThresholdMethods[method], commutative);
        }
        private static List<CorrelatedFeatures> GetNormal(Dictionary<string, List<float>> features, ThresholdFactory create, bool commutative = true)
        {
            List<CorrelatedFeatures> result = new List<CorrelatedFeatures>();
            List<String> orderedFeatures = new List<string>();
            foreach (var s in features.Keys) orderedFeatures.Add(s);
            for (int i = 0; i < orderedFeatures.Count; i++)
            {
                int mostCorrelative_idx = -1;
                float mostCorrelative_pearson = 0;

                string f1 = orderedFeatures[i];
                int j = commutative ? 0 : i + 1;
                for (; j < orderedFeatures.Count; j++)
                {
                    if (i == j) continue;
                    string f2 = orderedFeatures[j];
                    float[] x = features[f1].ToArray();
                    float[] y = features[f2].ToArray();
                    var p = MathUtil.Pearson(x, y);
                    if (Math.Abs(p) <= Math.Abs(mostCorrelative_pearson)) continue;
                    mostCorrelative_idx = j;
                    mostCorrelative_pearson = p;
                }
                if (mostCorrelative_idx == -1) continue;
                object[] rslt = create(features[f1].ToArray(), features[orderedFeatures[mostCorrelative_idx]].ToArray()); //<float threshold, string typeName, object correlationObject>
                if (rslt == null) continue;
                float mostCorrelative_threshold = (float)rslt[0] * 1.1f;
                string typeName = (string)rslt[1];
                object mostCorrelative_object = rslt[2];
                if (IsRegularNum(mostCorrelative_pearson) && IsRegularNum(mostCorrelative_threshold))
                    result.Add(new CorrelatedFeatures(f1, orderedFeatures[mostCorrelative_idx],
                         mostCorrelative_pearson, mostCorrelative_threshold, typeName, mostCorrelative_object));
            }
            return result;
        }


        public static Dictionary<string, List<int>> GetDetection(Dictionary<string, List<float>> features, List<CorrelatedFeatures> normal_model)
        {
            return GetDetection(features, normal_model, AnomalousChecker_hybrid);
        }
        private static Dictionary<string, List<int>> GetDetection(Dictionary<string, List<float>> features, List<CorrelatedFeatures> normal_model, AnomalousChecker checker)
        {
            Dictionary<string, List<int>> result = new Dictionary<string, List<int>>();
            foreach (var k in features.Keys) result.Add(k, new List<int>());
            foreach (CorrelatedFeatures n in normal_model)
            {
                string f1 = n.feature1;
                string f2 = n.feature2;
                List<float> x = features[f1];
                List<float> y = features[f2];
                for (int i = 0; i < x.Count; i++)
                {
                    if (checker(x[i], y[i], n))
                        result[f1].Add(i);
                }
            }
            return result;
        }


        private static bool AnomalousChecker_linear(float x, float y, CorrelatedFeatures c)
        {
            //if (c.lin_reg != null)
            //  Console.WriteLine(MathUtil.Dev(new Point() { x = x, y = y }, c.lin_reg));


            if (c.lin_reg != null)
                return MathUtil.Dev(new Point() { x = x, y = y }, c.lin_reg) > c.threshold;
            return false;
        }
        private static bool AnomalousChecker_circle(float x, float y, CorrelatedFeatures c)
        {
            if (c.minimal_circ == null) return false;
            return MinimalCircle.dist(new Point() { x = x, y = y }, c.minimal_circ.center) > c.threshold;
        }
        private static bool AnomalousChecker_hybrid(float x, float y, CorrelatedFeatures c)
        {
            return AnomalousChecker_linear(x, y, c) || AnomalousChecker_circle(x, y, c);
        }

        private static bool IsRegularNum(double num)
        {
            return double.IsFinite(num);
        }
        private static object[] ThresholdFactory_linear(float[] x, float[] y)
        {
            if (Math.Abs(MathUtil.Pearson(x, y)) < 0.9) return null;
            var tmp = MathUtil.Reg(x, y);
            if (!IsRegularNum(tmp.a) || !IsRegularNum(tmp.b)) return null;
            double max = 0;
            for (int i = 0; i < x.Length; i++)
            {
                Point p = new Point() { x = x[i], y = y[i] };
                max = Math.Max(max, Math.Abs(MathUtil.Dev(p, tmp)));
            }
            return new object[] { (float)max, "Line Regression", tmp };
        }
        private static object[] ThresholdFactory_circle(float[] x, float[] y)
        {
            if (Math.Abs(MathUtil.Pearson(x, y)) < 0.5) return null;
            var tmp = MathUtil.findMinCircle(x, y);
            if (!IsRegularNum(tmp.center.x) || !IsRegularNum(tmp.center.y) || !IsRegularNum(tmp.radius)) return null;
            return new object[] { tmp.radius, "Minimal Circle", tmp };
        }
        private static object[] ThresholdFactory_hybrid(float[] x, float[] y)
        {
            return ThresholdFactory_linear(x, y) ?? ThresholdFactory_circle(x, y);
        }


        public static Dictionary<string, List<Span>> ToSpanDictionary(Dictionary<string, List<int>> detection)
        {
            var spanDictionary = new Dictionary<string, List<Span>>();
            foreach (string f in detection.Keys)
            {
                var f_span = new List<Span>();
                long range_start = -100;
                long last_timestep = -100;
                foreach (var timeStep in detection[f])
                {
                    if (range_start == -100)
                    {
                        range_start = timeStep;
                        last_timestep = timeStep;
                        continue;
                    }
                    if (last_timestep + 1 == timeStep)
                    {
                        last_timestep = timeStep;
                        continue;
                    }
                    f_span.Add(new Span() { start = range_start, end = last_timestep });
                    last_timestep = timeStep;
                    range_start = timeStep;
                }
                if (range_start != -100)
                {
                    f_span.Add(new Span() { start = range_start, end = last_timestep });
                }
                spanDictionary.Add(f, f_span);
            }
            return spanDictionary;
        }

        public static Dictionary<string, string> GetReportTypes(List<CorrelatedFeatures> model, Dictionary<string, List<int>> detection)
        {
            Dictionary<string, string> reportTypes = new Dictionary<string, string>();
            foreach (var c in model)
                if (detection == null || (detection.ContainsKey(c.feature1) && detection[c.feature1].Count != 0))
                    reportTypes.TryAdd(c.feature1, c.typeName + " with " + c.feature2);
            return reportTypes;
        }
    }
}
