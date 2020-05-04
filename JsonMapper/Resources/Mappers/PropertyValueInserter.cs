using JsonMapper.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonMapper.Resources.Mappers
{
    /// <summary>
    /// This mapper class is pretty similar to propertyMapper, with one key difference:
    /// it takes a base string, and a replacementKey. Then it replaces contents of the provided path
    /// with the base string, in which the substring of replacementKey is replaced with the value read
    /// from the value list. Like so baseString = "Joku {replace} this is awesome" value = "jee" 
    /// --> "Joku jee this is awesome"
    /// </summary>
    internal class PropertyValueInserter : IMapper
    {
        private readonly string TargetPath;
        private readonly string SourceName;
        private readonly string TextToInsert;
        private readonly string ReplaceKey;
        private List<string> ValueList { get; set; }

        /// <summary>
        /// Constructs the class and reads necessary parameters.
        /// </summary>
        public PropertyValueInserter(Dictionary<string, string> parameters)
        {
            TargetPath = parameters["target"];
            SourceName = parameters["source"];
            TextToInsert = parameters["baseText"];
            ReplaceKey = parameters["replaceKey"];
        }
        /// <summary>
        /// Implements map-function.
        /// </summary>
        public void Map(List<JObject> templates)
        {
            if (ValueList == null || ValueList.Count == 0 || templates.Count == 0) return;
            MapValues(ValueList, templates);
        }

        /// <summary>
        /// Implements the value reading of dictionary.
        /// </summary>
        public void ReadTargetValues(Dictionary<string, List<string>> valueDictionary)
        {
            if (valueDictionary.Count == 0 || !valueDictionary.ContainsKey(SourceName)) return;
            ValueList = valueDictionary[SourceName];
        }

        /// <summary>
        /// Maps the values read into the target key and inserts read value into the 
        /// base string.
        /// </summary>
        private void MapValues(List<string> values, List<JObject> templates)
        {
            for (int i = 0; i < values.Count; i++)
            {
                if (i >= templates.Count) templates.Add((JObject)templates[0].DeepClone());
                JToken targetToken = templates[i].SelectToken(TargetPath);
                targetToken.Replace(TextToInsert.Replace(ReplaceKey, values[i].Trim()));
            }
        }
    }
}
