using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAgent.API;
using DataAgent.API.Monitoring.Intenral;

namespace DataAgent.Monitors.Interactions.UIA;
internal abstract class AutomationMonitor : MessagePublisher, IModule
{
    public AutomationMonitor()
    {
        WaitInBetweenEvents = 200;
    }
    public abstract void Initialize();
    public abstract void Shutdown();
}
