using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace JsonMapper.Interfaces
{
    /// <summary>
    /// Interface definition of the IMapper classes. These classes perform the different types of mapping functions defined
    /// in the parametrize syntax.
    /// </summary>
    interface IMapper
    {
        // Actual Mapping-function. Maps the list of JObjects, based on the template provided with values replaced
        // With the values read from the file.
        void Map(List<JObject> resultJsons);
        // Reads the afore-mentioned values and stores them inside the mapper class.
        void ReadTargetValues(Dictionary<string, List<string>> values);
    }
}
