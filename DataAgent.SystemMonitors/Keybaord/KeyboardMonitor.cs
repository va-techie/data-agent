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
using System.Windows.Input;

namespace DataAgent.Monitors.System.Keyboard;

internal class KeyboardMonitor : MessagePublisher, IMonitorKeyboard
{
    Dispatcher? dispatcher;
    IntPtr hookIDKeybaord = IntPtr.Zero;
    readonly ConcurrentQueue<Tuple<AutomationElement, IMonitorMessageExtendable>> automationElementsQueueForMoreDetails = new();
    public KeyboardMonitor()
    {
        this.name = Literals.KnownMonitors.Keyboard;
        this.displayName = Literals.KnownMonitors.Keyboard;
    }
    public void Initialize()
    {
        monitorProcessId = Process.GetCurrentProcess().Id;
        StartKeyboardHook();

        //isEnabled = false;
    }

    public void Shutdown()
    {
        if (hookIDKeybaord != IntPtr.Zero)
            Externs.UnhookWindowsHookEx(hookIDKeybaord);

        dispatcher?.BeginInvokeShutdown(DispatcherPriority.Input);
    }

    private void StartKeyboardHook()
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
                        hookIDKeybaord = Externs.SetWindowsHookEx(HookType.WH_KEYBOARD_LL, HookCallbackKeybaord, Externs.GetModuleHandle(curModule.ModuleName), 0);
                }

                Win.Threading.Dispatcher.Run();
            }

            catch (Exception ex)
            {
                PublishException(ex);
            }
        }));

        hooksThread.SetApartmentState(ApartmentState.STA);
        hooksThread.IsBackground = true;
        hooksThread.Start();
    }

    private IntPtr HookCallbackKeybaord(int nCode, IntPtr wParam, IntPtr lParam)
    {
        try
        {
            if (!isEnabled || nCode < 0)
                return Externs.CallNextHookEx(hookIDKeybaord, nCode, wParam, lParam);

            if (nCode >= 0 && wParam == (IntPtr)WindowsMessages.WM_KEYDOWN)
            {
                if (DateTime.Now - lastHandledEventTimeStamp > minWaitForNextEvent)
                {
                    lastHandledEventTimeStamp = DateTime.Now;

                    int vkCode = Marshal.ReadInt32(lParam);
                    Key key = KeyInterop.KeyFromVirtualKey(vkCode);

                    var msg = new MonitorMessage(lastHandledEventTimeStamp, this.Name)
                    {
                        Event = Literals.EventNames.KeyDown,
                    };

                    msg.SetPropertyValue(Literals.Properties.Names.Key, key.ToString());

                    bool queued = false;
                    bool sameProcess = false;
                    var focused = AutomationElement.FocusedElement;
                    if (focused != null)
                    {
                        msg.FocusedElementName = focused.Current.Name;
                        msg.FocusedElementType = focused.Current.LocalizedControlType;

                        int processId = focused.Current.ProcessId;
                        sameProcess = processId == monitorProcessId;
                        if (!sameProcess)
                        {
                            msg.ProcessId = processId;
                            var process = Diag.Process.GetProcessById(processId);
                            msg.ProcessName = process.ProcessName;
                            msg.WindowTitle = process.MainWindowTitle;
                        }

                        automationElementsQueueForMoreDetails.Enqueue(new Tuple<AutomationElement, IMonitorMessageExtendable>(focused, msg));
                        queued = true;
                        ThreadPool.QueueUserWorkItem((state) => AddDetailsToMessageFromQueueAndPublish());
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

        return Externs.CallNextHookEx(hookIDKeybaord, nCode, wParam, lParam);
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

}
