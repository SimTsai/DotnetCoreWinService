using System.Collections.Generic;

namespace DotnetCoreWinService
{
    public class ContainerConfig
    {
        public string DotnetPath { get; set; }
        public List<ContainerItemConfig> Configs { get; set; }
    }
}
