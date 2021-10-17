using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Defaults;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Windows.Media;

namespace StepsTracking.Model
{
    public class User
    {
        public string FullName { get; set; }
        public int AverageSteps { get; set; }
        public int BestResult { get; set; }
        public int WorstResult { get; set; }
        public List<UserDay> Days { get; set; }
        [JsonIgnore]
        public string Title { get; set; }
        [JsonIgnore]
        public Brush Color { get; set; }
        [JsonIgnore]
        public ChartValues<int> Values { get; set; }
        [JsonIgnore]
        public CartesianMapper<int> Mapper { get; set; }
    }
}
