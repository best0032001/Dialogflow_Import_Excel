using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaImport003.model
{
    public class DialogFlowInsertModel
    {
        public List<string> contexts { get; set; }
        public List<object> events { get; set; }
        public bool fallbackIntent { get; set; }
        public string name { get; set; }
        public int priority { get; set; }
        public List<Respons> responses { get; set; }
        public List<string> templates { get; set; }
        public List<UserSay> userSays { get; set; }
        public bool webhookForSlotFilling { get; set; }
        public bool webhookUsed { get; set; }
    }
    public class Parameters
    {
    }
    public class AffectedContext
    {
        public int lifespan { get; set; }
        public string name { get; set; }
        public Parameters parameters { get; set; }
    }
    public class DefaultResponsePlatforms
    {
        public bool google { get; set; }
    }
    public class Message
    {

        public string lang { get; set; }
        public object type { get; set; }
        public List<String> speech { get; set; }
    }
    public class Parameter
    {
        public string dataType { get; set; }
        public bool isList { get; set; }
        public string name { get; set; }
        public List<string> prompts { get; set; }
        public bool required { get; set; }
        public string value { get; set; }
    }

    public class Respons
    {
        public string action { get; set; }
        public List<AffectedContext> affectedContexts { get; set; }
        public DefaultResponsePlatforms defaultResponsePlatforms { get; set; }
        public List<Message> messages { get; set; }
        public List<Parameter> parameters { get; set; }
        public bool resetContexts { get; set; }
    }

    public class Data
    {
        public string alias { get; set; }
        public string meta { get; set; }
        public string text { get; set; }
        public bool userDefined { get; set; }
    }
    public class UserSay
    {
        public int count { get; set; }
        public List<Data> data { get; set; }
    }
}
