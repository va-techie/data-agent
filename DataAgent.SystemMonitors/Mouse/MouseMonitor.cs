using DataAgent.API;
using DataAgent.API.Monitoring;
using DataAgent.Monitors.System.Native;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Threading;
using Win = System.Windows;
using Diag = System.Diagnostics;
using DataAgent.API.Monitoring.Intenral;
using DataAgent.API.Monitoring.Intenral.Monitors;

namespace DataAgent.Monitors.System.Mouse;

internal class MouseMonitor : MessagePublisher, IMonitorMouse
{
    Dispatcher? dispatcher;
    IntPtr hookIDMouse = IntPtr.Zero;
    readonly ConcurrentQueue<Tuple<AutomationElement, IMonitorMessageExtendable>> automationElementsQueueForMoreDetails = new();
    public MouseMonitor()
    {
        this.name = Literals.KnownMonitors.Mouse;
        this.displayName = Literals.KnownMonitors.Mouse;
    }
    public void GetCursorPosition(out int x, out int y)
    {
        Externs.GetCursorPos(out POINT point);
        x = point.X;
        y = point.Y;
    }

    public void Initialize()
    {
        monitorProcessId = Process.GetCurrentProcess().Id;
        StartMouseHook();

        //isEnabled = false;
    }

    public void Shutdown()
    {
        if (hookIDMouse != IntPtr.Zero)
            Externs.UnhookWindowsHookEx(hookIDMouse);

        dispatcher?.BeginInvokeShutdown(DispatcherPriority.Input);
    }

    private void StartMouseHook()
    {
        Thread hooksThread = new Thread(new ThreadStart(() =>
        {
            try
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
                dispatcher = Dispatcher.CurrentDispatcher;

                using (Process curProcess = Process.GetCurrentProcess())
                using (ProcessModule? curModule = curProcess.MainModule)
                {
                    if (curModule != null && curModule.ModuleName != null)
                        hookIDMouse = Externs.SetWindowsHookEx(HookType.WH_MOUSE_LL, HookCallbackMouse, Externs.GetModuleHandle(curModule.ModuleName), 0);
                }

                Win.Threading.Dispatcher.Run();
            }

            catch (Exception ex)
            {
                PublishException(ex);
            }
            finally
            {

            }
        }));

        hooksThread.SetApartmentState(ApartmentState.STA);
        hooksThread.IsBackground = true;
        hooksThread.Start();
    }

    private IntPtr HookCallbackMouse(int nCode, IntPtr wParam, IntPtr lParam)
    {
        try
        {
            if (!isEnabled || nCode < 0)
                return Externs.CallNextHookEx(hookIDMouse, nCode, wParam, lParam);

            if (wParam == (IntPtr)WindowsMessages.WM_LBUTTONDOWN || wParam == (IntPtr)WindowsMessages.WM_RBUTTONDOWN)
            {
                if (DateTime.Now - lastHandledEventTimeStamp > MinWaitForNextEvent)
                {
                    lastHandledEventTimeStamp = DateTime.Now;
                    Externs.GetCursorPos(out POINT point);

                    var msg = new MonitorMessage(lastHandledEventTimeStamp, this.Name)
                    {
                        Event = (wParam == (IntPtr)WindowsMessages.WM_LBUTTONDOWN) ? Literals.EventNames.MouseLeft : Literals.EventNames.MouseRight,
                        CursorPosition = new Point(point.X, point.Y)
                    };

                    var element = GetElementFromPoint(point);

                    bool queued = false;
                    bool sameProcess = false;
                    if (element != null)
                    {
                        int processId = element.Current.ProcessId;
                        sameProcess = processId == monitorProcessId;

                        if (!sameProcess)
                        {
                            msg.ProcessId = processId;
                            var process = Diag.Process.GetProcessById(processId);
                            msg.ProcessName = process.ProcessName;
                            msg.WindowTitle = process.MainWindowTitle;

                            msg.InputElementType = element.Current.LocalizedControlType;
                            msg.InputElementName = element.Current.Name;

                            var focused = AutomationElement.FocusedElement;
                            msg.FocusedElementName = focused.Current.Name;
                            msg.FocusedElementType = focused.Current.LocalizedControlType;

                            automationElementsQueueForMoreDetails.Enqueue(new Tuple<AutomationElement, IMonitorMessageExtendable>(element, msg));
                            queued = true;
                            ThreadPool.QueueUserWorkItem((state) => AddDetailsToMessageFromQueueAndPublish());
                        }
                    }

                    if (!sameProcess && !queued)
                        Publish(msg);
                }
            }

        }
        catch (Exception ex)
        {
            PublishException(ex);
        }

        return Externs.CallNextHookEx(hookIDMouse, nCode, wParam, lParam);
    }

    private void AddDetailsToMessageFromQueueAndPublish()
    {
        try
        {
            while (automationElementsQueueForMoreDetails.TryDequeue(out var item))
            {
                try
                {
                    if (isEnabled)
                    {
                        var element = item.Item1;
                        var msg = item.Item2;

                        //Add more details from automation as required.

                        Publish(msg);
                    }
                }
                catch (Exception ex) { PublishException(ex); }
            }
        }
        catch (Exception ex) { PublishException(ex); }
    }
    private static AutomationElement GetElementFromPoint(POINT point)
    {
        AutomationElement element = AutomationElement.FromPoint(new Win.Point(point.X, point.Y));
        return element;
    }

}
