﻿using DataAgent.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAgent.Monitors.System.Keyboard;

public class Module : IModule
{
    readonly KeyboardMonitor mouseMonitor = new();
    public void Initialize()
    {            
        API.Services.Providers.AddPublisherByFramework(mouseMonitor.Name, mouseMonitor);
        mouseMonitor.Initialize();
    }

    public void Shutdown()
    {
        mouseMonitor.Shutdown();
        API.Services.Providers.RemovePublisherByFramework(mouseMonitor);
    }
}
