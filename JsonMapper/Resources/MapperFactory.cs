using JsonMapper.Interfaces;
using JsonMapper.Resources.Mappers;
using System.Collections.Generic;
using System.Linq;

namespace JsonMapper.Resources
{
    /// <summary>
    /// This class creates the mapper classes based on the instructions.
    /// All of these classes implement the interface of IMapper.
    /// </summary>
    internal class MapperFactory
    {
        /// <summary>
        /// Generates the actual mapper objects and adds them to the List of different mapper-classes.
        /// </summary>
        internal List<IMapper> GenerateMappers(List<string> testInstructions)
        {
            List<IMapper> mapperClass = new List<IMapper>();
            testInstructions.ForEach(instruction => mapperClass.Add(SeparateSyntax(instruction)));
            return mapperClass;
        }

        /// <summary>
        /// Separates the parametrized syntax by following the defined splitter, in this case (.
        /// as in, mapperClass(...) -> mapperClass (.....)
        /// </summary>
        private IMapper SeparateSyntax(string instruction)
        {
            string[] syntax = instruction.Split(new char[] { '(' });
            Dictionary<string, string> mapInstructions = InterpretProperties(syntax[1].Replace(")", ""));
            IMapper mapperClass = FindAndGenerateMapperClass(syntax[0], mapInstructions);
            return mapperClass;
        }

        /// <summary>
        /// Splits the different commands by ',' and adds them to parameter dictionary.
        /// As an example, target=something, valueColumn=smthnElse -> Dict<<target:something>,<valueColumn:smthnElse>>
        /// </summary>
        private Dictionary<string, string> InterpretProperties(string syntax)
        {
            Dictionary<string, string> parameterDictionary = new Dictionary<string, string>();
            if (!syntax.Contains("source")) return parameterDictionary;
            string[] differentCommands = syntax.Split(',');
            differentCommands.ToList().ForEach(cmd => FindAndSetProperty(cmd.Trim(), parameterDictionary));
            return parameterDictionary;
        }

        /// <summary>
        /// Splits the commands by '=' char and adds them to the dictionary
        /// </summary>
        private void FindAndSetProperty(string singleInstruction, Dictionary<string, string> parameterDictionary)
        {
            List<string> commandAndValue = singleInstruction.Split('=').ToList();
            commandAndValue.ForEach(x => {
                x = x
                    .Replace("=", "")
                    .Replace("\"", "");
                });
            parameterDictionary[commandAndValue[0]] = commandAndValue[1].Trim('"');
        }

        /// <summary>
        /// After interpreting parameters and classNames, creates a class that implements the IMapper
        /// - interface and passes the parsed parameters to it.
        /// </summary>
        private IMapper FindAndGenerateMapperClass(string className, Dictionary<string, string> parameters)
        {
            IMapper mapperToReturn = null;
            if (parameters is null || parameters.Count == 0) return mapperToReturn;

            switch (className)
            {
                case "PropertyMap":
                        mapperToReturn = new PropertyMapper(parameters);
                        break;
                case "SiblingPropertyMap":
                        mapperToReturn = new SiblingPropertyMapper(parameters);
                        break;
                case "PropertyValueInserter":
                        mapperToReturn = new PropertyValueInserter(parameters);
                        break;
                case "SiblingColumnPropertyMap":
                    mapperToReturn = new SiblingPropertyColumnMapper(parameters);
                    break;
                case "PropertyValueExcluder":
                    mapperToReturn = new PropertyValueExcluder(parameters);
                    break;
            }
            return mapperToReturn;
        }
    }
}