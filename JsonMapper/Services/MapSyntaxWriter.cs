namespace JsonMapper.Services
{
    internal class MapSyntaxWriter
    {
        internal string WritePropertyMap(string targetName, string sourceName)
        {
            return $"PropertyMap(target=\"{targetName}\", source=\"{sourceName}\")";
        }

        internal string WriteSiblingPropertyMap(string targetName, string sourceName, string siblingName, string siblingValue = "")
        {
            return $"SiblingPropertyMap(target=\"{targetName}\", source=\"{sourceName}\", siblingName=\"{siblingName}\", siblingValue=\"{siblingValue}\")";
        }

        internal string WriteSiblingColumnPropertyMap(string targetName, string sourceName, string siblingColumn, string siblingValue)
        {
            return $"SiblingColumnPropertyMap(target=\"{targetName}\", source=\"{sourceName}\", siblingColumn=\"{siblingColumn}\", siblingValue=\"{siblingValue}\")";
        }
        internal string WritePropertyValueInserter(string targetName, string sourceName, string baseText, string replaceKey)
        {
            return $"PropertyValueInserter(target=\"{targetName}\", source=\"{sourceName}\", baseText=\"{baseText}\", replaceKey=\"{replaceKey}\")";
        }
        internal string WritePropertyValueExcluder(string propertyColumn, string valueRequirement)
        {
            return $"PropertyValueExcluder(source=\"{propertyColumn}\", valueRequirement=\"{valueRequirement}\")";
        }
    }
}