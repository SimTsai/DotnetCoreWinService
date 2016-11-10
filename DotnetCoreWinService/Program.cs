using System.ServiceProcess;

namespace DotnetCoreWinService
{
    static class Program
    {
        static void Main()
        {
#if DEBUG && !SERVICE
            var service = new ContainerService();
            service.OnStart();
#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new ContainerService()
            };
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
