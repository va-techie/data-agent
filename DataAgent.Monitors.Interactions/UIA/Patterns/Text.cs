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
internal class Text : AutomationMonitor
{
    const int maxTextLength = 1000;
    public Text()
    {
        this.name = Literals.Patterns.Text;
        this.displayName = "Text Pattern";
    }
    public override void Initialize()
    {
        monitorProcessId = Process.GetCurrentProcess().Id;
        //minWaitForNextEvent = TimeSpan.FromMilliseconds(1000);
        Automation.AddAutomationEventHandler(TextPattern.TextChangedEvent,
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
            if (element == null || element.Current.IsOffscreen)
                return;

            var processId = element.Current.ProcessId;
            if (processId == monitorProcessId)
                return;

            var msg = new MonitorMessage(lastHandledEventTimeStamp, this.DisplayName)
            {
                Event = Literals.Patterns.Text,
            };

            msg.ProcessId = processId;
            var process = Process.GetProcessById(processId);
            msg.ProcessName = process.ProcessName;
            msg.WindowTitle = process.MainWindowTitle;

            msg.InputElementType = element.Current.LocalizedControlType;
            msg.InputElementName = element.Current.Name;

            if (element.TryGetCurrentPattern(TextPattern.Pattern, out var pattern))
            {
                if (pattern is TextPattern textPattern)
                {
                    TextPatternRange textRange = textPattern.DocumentRange;
                    var newText = textRange?.GetText(maxTextLength);

                    msg.SetPropertyValue(Literals.Properties.Names.Value, newText);
                }
            }

            this.PublishNoHook(msg);
        }
        catch (Exception ex) { PublishException(ex); }

    }
    public override void Shutdown()
    {
        Automation.RemoveAutomationEventHandler(TextPattern.TextChangedEvent,
            AutomationElement.RootElement,
            OnAutomationEvent);
    }
}
