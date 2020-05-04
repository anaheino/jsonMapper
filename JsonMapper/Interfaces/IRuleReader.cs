using System.Collections.Generic;

namespace JsonMapper.Interfaces
{
    internal interface IRuleReader
    {
        List<string> LoadRules();
        void SaveRules(List<string> rulesToSave);
    }
}