using JsonMapper.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JsonMapper.Resources
{
    internal class CsvValueReader : IValueReader
    {
        /// <summary>
        /// Reads values stored in the csv column corresponding with the targetProperty
        /// </summary>
        public Dictionary<string, List<string>> Read(string filePath, char separator)
        {
            Dictionary<string, List<string>> valueDictionary = new Dictionary<string, List<string>>();

            using (var reader = new StreamReader(filePath))
            {
                string firstLine = reader.ReadLine();
                string[] headers = firstLine.Split(separator);
                // Let's make a new list for each column header
                int index = 0;
                Dictionary<int, string> valueByIndex = new Dictionary<int, string>();
                headers.ToList().ForEach(head =>
                {
                    if (valueDictionary.ContainsKey(head)) head += index.ToString();
                    valueDictionary.Add(head, new List<string>());
                    valueByIndex.Add(index, head);
                    index++;
                });
                // Still needs to handle specific cases, like head containing this already.
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] lineValues = line.Split(separator);
                    for (int i = 0; i < lineValues.Length; i++)
                    {
                        valueDictionary[valueByIndex[i]].Add(lineValues[i]);
                    }
                }
            }
            return valueDictionary;
        }
    }
}