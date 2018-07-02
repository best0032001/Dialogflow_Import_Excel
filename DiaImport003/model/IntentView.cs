using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaImport003.model
{
    public class IntentView
    {
        public IntentView()
        {
            name = "-";
            action = "-";
            input = new List<string>();
            response = new List<string>();
        }
        public String name { get; set; }
        public String action { get; set; }
        public List<String> input { get; set; }
        public List<String> response { get; set; }
    }
}
