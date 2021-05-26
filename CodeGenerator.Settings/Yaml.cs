using System;
using CodeGenerator.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CodeGenerator.Settings
{
    public static class Yaml
    {
        public static BuildDefinition ParseYaml(string yaml)
        {
            var deserializer = getDeserializer();
            return deserializer.Deserialize<BuildDefinition>(yaml);
        }

        private static IDeserializer getDeserializer()
        {
            return new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)  // see height_in_inches in sample yml 
                .IgnoreUnmatchedProperties()
                .Build();
        }
    }
}
