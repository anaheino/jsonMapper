using System.Collections.Generic;
using JsonMapper.Interfaces;
using JsonMapper.Resources;
using JsonMapper.Resources.Mappers;
using Newtonsoft.Json.Linq;

namespace JsonMapper.Services
{
    /// <summary>
    /// The master class for mapping values into separate objects.
    /// This class is used for creating and calling the mapper-classes for specific mappings.
    /// It's the service level which the UI uses.
    /// </summary>
    public class ValueMapper
    {
        private List<IMapper> _mappers { get; set; }
        private JObject _template { get; set; }
        public List<string> NodeValues { get; private set; }
        private readonly MapperFactory factory;
        private readonly PropertyReader propReader;
        private readonly CsvValueReader valueReader;
        internal Dictionary<string, List<string>> valuesByColumnName;

        public ValueMapper()
        {
            factory = new MapperFactory();
            propReader = new PropertyReader();
            valueReader = new CsvValueReader();
        }
        /// <summary>
        /// Generates the mapper classes based on the syntax provided by the calling function.
        /// </summary>
        public void SetInstructions(List<string> testInstructions)
        {
            _mappers = factory.GenerateMappers(testInstructions);
        }
        /// <summary>
        /// Reads the values from the provided file name.
        /// </summary>
        public void SetValues(string pathToValues, char separator)
        {
            valuesByColumnName = valueReader.Read(pathToValues, separator);
        }

        /// <summary>
        /// Sets the JObject template to replicate.
        /// </summary>
        public void SetTemplate(JObject testTemplate)
        {
            _template = testTemplate;
            NodeValues = propReader.MapChildren(_template);
        }

        /// <summary>
        /// Replicates the data from the JSONS.
        /// </summary>
        public List<JObject> MapValues()
        {
            List<JObject> resultObjects = new List<JObject>() { _template};
            _mappers.ForEach(map => map.ReadTargetValues(valuesByColumnName));
            _mappers.ForEach(map => map.Map(resultObjects));
            return resultObjects;
        }
    }
}
