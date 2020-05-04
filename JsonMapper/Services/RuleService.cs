using JsonMapper.Interfaces;
using JsonMapper.Resources;
using System.Collections.Generic;

namespace JsonMapper.Services
{
    internal class RuleService
    {
        private readonly IRuleReader ruleReader;

        public RuleService()
        {
            ruleReader = new WindowsRuleReader();
        }

        internal List<string> LoadRules()
        {
            return ruleReader.LoadRules();
        }

        internal void SaveRules(List<string> rulesToSave)
        {
            ruleReader.SaveRules(rulesToSave);
        }
    }
}