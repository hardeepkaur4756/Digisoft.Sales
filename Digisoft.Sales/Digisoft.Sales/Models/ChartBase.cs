using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Digisoft.Sales.Models
{
    public class ChartBase
    {
        public List<string> Labels { get; set; }
        public List<Datasets> Datasets { get; set; }
    }
    public class Datasets
    {
        public List<int> data { get; set; }
        public string backgroundColor { get; set; }

        public bool fill { get; set; }
        public string borderColor { get; set; }
        public string label { get; set; }
    }
}