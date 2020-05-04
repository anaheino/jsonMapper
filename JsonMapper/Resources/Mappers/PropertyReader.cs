using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace JsonMapper.Resources.Mappers
{
    /// <summary>
    /// This class exists to find all properties in a JSON template-object.
    /// This is mostly so in the ui you can select the properties from a list.
    /// </summary>
    internal class PropertyReader
    {
        /// <summary>
        /// Maps the children properties in the template object.
        /// </summary>
        public List<string> MapChildren(JObject template)
        {
            List<string> childrenValues = new List<string>();
            MapPropertiesRecursively(template, childrenValues);
            return childrenValues;
        }

        /// <summary>
        /// Recursive function which seeks all the JProperties in the template.
        /// </summary>
        private void MapPropertiesRecursively(JToken template, List<string> childrenValues)
        {
            if (template is null) return;
            if (template is JProperty daProperty)
            {
                childrenValues.Add(daProperty.Path);
            }
            List<JToken> childrenList = template.Children().ToList();
            childrenList.ForEach(child => MapPropertiesRecursively(child, childrenValues));
        }
    }
}