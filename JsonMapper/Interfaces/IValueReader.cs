using System.Collections.Generic;

namespace JsonMapper.Interfaces
{
    internal interface IValueReader
    {
        Dictionary<string,List<string>> Read(string filePath, char separator);
    }
}