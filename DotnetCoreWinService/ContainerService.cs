using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace DotnetCoreWinService
{
    public partial class ContainerService : ServiceBase
    {
        string dotnetPath;
        List<Process> processes = new List<Process>();

        public ContainerService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            this.OnStart();
        }

        internal void OnStart()
        {
#if DEBUG && SERVICE
            SpinWait.SpinUntil(() => Debugger.IsAttached);
#endif
            var configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
            var configJson = File.ReadAllText(configFilePath);
            var config = JsonConvert.DeserializeObject<ContainerConfig>(configJson);
            if (!CheckDotnetAvailable(config))
            {
                this.Stop();
            }
            var tasks = new List<Task<Process>>();
            config.Configs.ForEach(cfg =>
            {
                var task = CreateTask(cfg);
                tasks.Add(task);
                task.Start();
            });
            Task.WaitAll(tasks.ToArray());
            tasks.ForEach(p => processes.Add(p.Result));
        }

        bool CheckDotnetAvailable(ContainerConfig config)
        {
            using (Process dotnetProcess = new Process())
            {
                this.dotnetPath = config.DotnetPath;
                if (string.IsNullOrWhiteSpace(dotnetPath))
                {
                    dotnetPath = "dotnet.exe";
                }
                ProcessStartInfo psi = new ProcessStartInfo(dotnetPath, "--version");
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                dotnetProcess.StartInfo = psi;
                if (!dotnetProcess.Start())
                {
                    return false;
                }
                dotnetProcess.WaitForExit();
                var output = dotnetProcess.StandardOutput.ReadToEnd();
                if (string.IsNullOrWhiteSpace(output))
                {
                    return false;
                }
                return true;
            }
        }

        Task<Process> CreateTask(ContainerItemConfig cfg)
        {
            Task<Process> task = new Task<Process>(() =>
            {
                Process dotnetProcess = new Process();
                ProcessStartInfo psi = new ProcessStartInfo(dotnetPath, $"{cfg.AssemblyPath} {cfg.Parameter}");
                if (!string.IsNullOrWhiteSpace(cfg.WorkDirectory))
                {
                    try
                    {
                        if (!Directory.Exists(cfg.WorkDirectory))
                        {
                            Directory.CreateDirectory(cfg.WorkDirectory);
                        }
                    }
                    catch
                    {
                        dotnetProcess.Dispose();
                        return null;
                    }
                    psi.WorkingDirectory = cfg.WorkDirectory;
                }
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                dotnetProcess.StartInfo = psi;
                if (!dotnetProcess.Start())
                {
                    dotnetProcess.Dispose();
                    return null;
                }
                return dotnetProcess;
            });

            return task;
        }


        protected override void OnStop()
        {
            processes.ForEach(process =>
            {
                process.Kill();
                process.Dispose();
            });
        }
    }
}
