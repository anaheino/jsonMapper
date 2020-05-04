using JsonMapper.Interfaces;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace JsonMapper.Resources.Mappers
{
    internal class PropertyMapper : IMapper
    {
        public readonly string TargetPath;
        public readonly string SourceName;

        private List<string> valueList { get; set; }
        
        /// <summary>
        /// This constructor defines the mandatory parameters required for the propertyMapper-class,
        /// the most simple one of the IMapper classes.
        /// </summary>
        public PropertyMapper(Dictionary<string, string> parameters)
        {
            TargetPath = parameters["target"];
            SourceName = parameters["source"];
        }

        /// <summary>
        /// Implements the Map-function of IMapper interface.
        /// </summary>
        public void Map(List<JObject> jsonObjects)
        {
            if (valueList == null || valueList.Count == 0 || jsonObjects.Count == 0) return;
            MapValues(valueList, jsonObjects);
        }

        /// <summary>
        /// Calls the IValueReader to find the parametrized values from the .csv-file and return them
        /// as a List of strings.
        /// </summary>
        public void ReadTargetValues(Dictionary<string, List<string>> valueDictionary)
        {
            if (valueDictionary.Count == 0 || !valueDictionary.ContainsKey(SourceName)) return;
            valueList = valueDictionary[SourceName];
        }

        /// <summary>
        /// Clones the JObject that has been given to it, and 
        /// sets the value of the targetted path to the value stored in value-list.
        /// Then it adds it to the List of JObjects, and finally returns the List of JObjects.
        /// </summary>
        private void MapValues(List<string> values, List<JObject> resultJsons)
        {
            for (int i = 0; i < values.Count; i++)
            {
                if (i >= resultJsons.Count) resultJsons.Add((JObject)resultJsons[0].DeepClone());
                JToken targetToken = resultJsons[i].SelectToken(TargetPath);
                targetToken.Replace(values[i].Trim());
            }
        }
    }
}