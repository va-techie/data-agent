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

namespace DataAgent.Monitors.Interactions.UIA.Patterns;
internal class Toggle : AutomationMonitor
{
    public Toggle()
    {
        this.name = Literals.Patterns.Toggle;
        this.displayName = "Toggle Pattern";
    }
    public override void Initialize()
    {
        monitorProcessId = Process.GetCurrentProcess().Id;

        Automation.AddAutomationPropertyChangedEventHandler(AutomationElement.RootElement,
            TreeScope.Subtree, OnAutomationTogglePropertyChanged, TogglePattern.ToggleStateProperty);
    }
    private void OnAutomationTogglePropertyChanged(object sender, AutomationPropertyChangedEventArgs e)
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
                Event = Literals.Patterns.Toggle,
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

            msg.SetPropertyValue(Literals.Properties.Names.Value, e.NewValue?.ToString());

            this.PublishNoHook(msg);
        }
        catch (Exception ex)
        {
            PublishException(ex);
        }

    }
    public override void Shutdown()
    {
        Automation.RemoveAutomationPropertyChangedEventHandler(AutomationElement.RootElement,
            OnAutomationTogglePropertyChanged);
    }
}
