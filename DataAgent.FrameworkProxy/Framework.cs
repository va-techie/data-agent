using DataAgent.API;
using DataAgent.API.Services.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAgent.FrameworkProxy;

public static class Framework
{
    static readonly IList<IModule> monitorModules = new List<IModule>();
    public static void Initialize()
    {
        monitorModules.Add(new DataAgent.Monitors.System.Mouse.Module());
        monitorModules.Add(new DataAgent.Monitors.System.Keyboard.Module());
        monitorModules.Add(new DataAgent.Monitors.Interactions.UIA.Module());

        foreach (var module in monitorModules)
            module.Initialize();

    }

    public static void Shutdown()
    {
        foreach (IModule module in monitorModules)
        {
            module.Shutdown();
            if (module is IDisposable disposable)
                disposable.Dispose();
        }

        monitorModules.Clear();
    }
}
