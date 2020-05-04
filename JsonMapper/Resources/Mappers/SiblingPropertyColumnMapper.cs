using System.Collections.Generic;
using System.Linq;
using JsonMapper.Interfaces;
using Newtonsoft.Json.Linq;

namespace JsonMapper.Resources.Mappers
{
    /// <summary>
    /// Implements IMapper class. Maps values to the target property,
    /// if target property has a sibling and it's value matches parametrized sibling-value.
    /// Similar to siblingMapper, except takes into account the sibling column value instead of template
    /// value.
    /// </summary>
    internal class SiblingPropertyColumnMapper : IMapper
    {
        public readonly string TargetPath;
        public readonly string SourceName;
        public readonly string SiblingValue;
        public readonly string SiblingColumnName;
        private List<string> valueList;
        private List<string> siblingColumnValueList;


        /// <summary>
        /// Simple constructor that reads the target and source values from parameter dictionary.
        /// </summary>
        public SiblingPropertyColumnMapper(Dictionary<string, string> parameters)
        {
            TargetPath = parameters["target"];
            SourceName = parameters["source"];
            SiblingColumnName = parameters["siblingColumn"];
            SiblingValue = parameters["siblingValue"];
        }

        /// <summary>
        /// Implements map-function to map provided values.
        /// </summary>
        public void Map(List<JObject> templates)
        {
            if (valueList == null || valueList.Count == 0 || templates.Count == 0) return;
            MapValues(valueList, siblingColumnValueList, templates);
        }
        
        /// <summary>
        /// Does the actual mapping. Uses the parameters to replace target value, if siblingPath exists and
        /// siblingValue is either blank or matches the provided sibling value.
        /// </summary>
        private void MapValues(List<string> values, List<string> siblingColumnValues, List<JObject> valList)
        {
            for (int i = 0; i < values.Count; i++)
            {
                if (i >= valList.Count) valList.Add((JObject)valList[0].DeepClone());
                JToken valueToken = valList[i].SelectToken(TargetPath);
                if (string.Equals(siblingColumnValues[i].Trim(), SiblingValue.Trim(), System.StringComparison.InvariantCultureIgnoreCase) || string.IsNullOrEmpty(SiblingValue))
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
            siblingColumnValueList = valueDictionary[SiblingColumnName];
        }
    }
}