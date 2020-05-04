using System.Collections.Generic;
using System.Linq;
using JsonMapper.Interfaces;
using Newtonsoft.Json.Linq;

namespace JsonMapper.Resources.Mappers
{
    /// <summary>
    /// Implements IMapper class. Maps values to the target property,
    /// if target property has a sibling and it's value matches parametrized sibling-value.
    /// </summary>
    internal class SiblingPropertyMapper : IMapper
    {
        public readonly string TargetPath;
        public readonly string SourceName;
        public readonly string SiblingName;
        public readonly string SiblingValue;
        private List<string> valueList;


        /// <summary>
        /// Simple constructor that reads the target and source values from parameter dictionary.
        /// </summary>
        public SiblingPropertyMapper(Dictionary<string, string> parameters)
        {
            TargetPath = parameters["target"];
            SourceName = parameters["source"];
            SiblingName = parameters["siblingName"];
            SiblingValue = parameters["siblingValue"];
        }

        /// <summary>
        /// Implements map-function to map provided values.
        /// </summary>
        public void Map(List<JObject> templates)
        {
            if (valueList == null || valueList.Count == 0 || templates.Count == 0) return;
            MapValues(valueList, templates);
        }
        
        /// <summary>
        /// Does the actual mapping. Uses the parameters to replace target value, if siblingPath exists and
        /// siblingValue is either blank or matches the provided sibling value.
        /// </summary>
        private void MapValues(List<string> values, List<JObject> valList)
        {
            for (int i = 0; i < values.Count; i++)
            {
                if (i >= valList.Count) valList.Add((JObject)valList[0].DeepClone());
                string[] partOfPath = TargetPath.Split('.');
                partOfPath[partOfPath.Length - 1] = SiblingName;
                string siblingPath = "";
                partOfPath.ToList().ForEach(path => {
                    if (string.IsNullOrEmpty(siblingPath)) siblingPath = path;
                    else siblingPath += "." + path;
                    });
                JToken valueToken = valList[i].SelectToken(TargetPath);
                string siblingVal = (string)valList[i].SelectToken(siblingPath);
                 
                if (siblingVal.Equals(SiblingValue) || string.IsNullOrEmpty(SiblingValue))
                    valueToken.Replace(values[i].Trim());
            }
        }

        /// <summary>
        /// Reads the provided values from dictionary.
        /// </summary>
        public void ReadTargetValues(Dictionary<string, List<string>> valueDictionary)
        {
            if (valueDictionary.Count == 0 || !valueDictionary.ContainsKey(SourceName)) return;
            valueList = valueDictionary[SourceName];
        }
    }
}