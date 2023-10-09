using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using DataAgent.API;

namespace DataAgent.Monitors.Interactions.UIA;

public class Module : IModule
{
    readonly List<AutomationMonitor> monitors = new();
    readonly ManualResetEvent exitEvent = new(false);
    readonly ManualResetEvent moduleExitCompletd = new(false);
    readonly ManualResetEvent publishersAdded = new(false);

    public ManualResetEvent ExitEvent => exitEvent;

    public void Initialize()
    {
        var thread = new Thread(new ThreadStart(() =>
            {
                try
                {
                    //Add automation monitors
                    monitors.Add(new Patterns.Toggle());
                    monitors.Add(new Patterns.Text());
                    monitors.Add(new Patterns.Invoke());

                    //Register publishers
                    foreach (var monitor in monitors)
                    {
                        monitor.Initialize();
                        API.Services.Providers.AddPublisherByFramework(monitor.Name, monitor);
                    }

                    publishersAdded.Set();

                    //Wait for exit trigger
                    ExitEvent.WaitOne();

                    //Remove all automation monitors
                    foreach (var monitor in monitors)
                    {
                        monitor.Shutdown();
                        API.Services.Providers.RemovePublisherByFramework(monitor);
                    }
                    monitors.Clear();

                }
                finally
                {
                    Automation.RemoveAllEventHandlers();
                    moduleExitCompletd.Set();
                }
            }));

        thread.SetApartmentState(ApartmentState.MTA);
        thread.IsBackground = true;
        thread.Start();

        publishersAdded.WaitOne();
    }

    public void Shutdown()
    {
        ExitEvent.Set();
        moduleExitCompletd.WaitOne(5000);
    }
}
