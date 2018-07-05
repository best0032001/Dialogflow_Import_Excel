using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaImport003.model
{
    public class InentModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public List<object> contextIn { get; set; }
        public List<object> events { get; set; }
        public List<object> parameters { get; set; }
        public List<object> contextOut { get; set; }
        public List<string> actions { get; set; }
        public int priority { get; set; }
        public bool fallbackIntent { get; set; }
    }
}
