using System.Collections.Generic;

namespace DotnetCoreWinService
{
    public class ContainerItemConfig
    {
        public string AssemblyPath { get; set; }
        public string WorkDirectory { get; set; }
        public string Parameter { get; set; }
        public List<KeyValuePair<string, string>> EnvironmentVariables { get; set; }
    }
}
