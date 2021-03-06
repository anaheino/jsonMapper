﻿using JsonMapper.Interfaces;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace JsonMapper.Resources.Mappers
{
    internal class PropertyValueExcluder : IMapper
    {
        public readonly string ValueRequirement;
        public readonly string SourceName;

        private List<string> valueList { get; set; }
        
        /// <summary>
        /// This constructor defines the mandatory parameters required for the propertyMapper-class,
        /// the most simple one of the IMapper classes.
        /// </summary>
        public PropertyValueExcluder(Dictionary<string, string> parameters)
        {
            ValueRequirement = parameters["valueRequirement"];
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
        /// Removes all JSONS from the JObject list that don't match the provided valueRequirement
        /// </summary>
        private void MapValues(List<string> values, List<JObject> resultJsons)
        {
            List<JObject> actualRequirements = new List<JObject>();
            for (int i = 0; i < values.Count; i++)
            {
                if (i >= resultJsons.Count) resultJsons.Add((JObject)resultJsons[0].DeepClone());
                if (values[i].Trim().Equals(ValueRequirement.Trim()))
                {
                    actualRequirements.Add(resultJsons[i]);
                }
            }
            resultJsons.RemoveAll(token => !actualRequirements.Contains(token));
        }
    }
}