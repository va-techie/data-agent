using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using DataAgent.API.Monitoring.Intenral;
using DataAgent.API;
using System.Diagnostics;
using System.Windows.Automation.Text;

namespace DataAgent.Monitors.Interactions.UIA.Patterns;
internal class Invoke : AutomationMonitor
{
    public Invoke()
    {
        this.name = Literals.Patterns.Invoke;
        this.displayName = "Invoke Pattern";
    }
    public override void Initialize()
    {
        monitorProcessId = Process.GetCurrentProcess().Id;
        //minWaitForNextEvent = TimeSpan.FromMilliseconds(500);
        Automation.AddAutomationEventHandler(InvokePattern.InvokedEvent,
            AutomationElement.RootElement,
            TreeScope.Subtree, OnAutomationEvent);
    }

    private void OnAutomationEvent(object sender, AutomationEventArgs e)
    {
        try
        {
            if (!isEnabled) return;

            if (DateTime.Now - lastHandledEventTimeStamp < minWaitForNextEvent)
                return;

            lastHandledEventTimeStamp = DateTime.Now;

            AutomationElement? element = sender as AutomationElement;
            if (element == null)
                return;

            var processId = element.Current.ProcessId;
            if (processId == monitorProcessId)
                return;

            var msg = new MonitorMessage(lastHandledEventTimeStamp, this.DisplayName)
            {
                Event = Literals.Patterns.Invoke,
            };

            msg.ProcessId = processId;
            var process = Process.GetProcessById(processId);
            msg.ProcessName = process.ProcessName;
            msg.WindowTitle = process.MainWindowTitle;

            msg.InputElementType = element.Current.LocalizedControlType;
            msg.InputElementName = element.Current.Name;

            var focused = AutomationElement.FocusedElement;
            if (focused != null)
            {
                msg.FocusedElementName = focused.Current.Name;
                msg.FocusedElementType = focused.Current.LocalizedControlType;
            }

            this.PublishNoHook(msg);
        }
        catch (Exception ex) { PublishException(ex); }

    }
    public override void Shutdown()
    {
        Automation.RemoveAutomationEventHandler(InvokePattern.InvokedEvent,
            AutomationElement.RootElement,
            OnAutomationEvent);
    }
}
