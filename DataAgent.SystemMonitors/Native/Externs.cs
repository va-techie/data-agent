using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DataAgent.Monitors.System.Native;

internal static class Externs
{
    [DllImport("user32.dll")]
    public static extern bool GetCursorPos(out POINT lpPoint);

    public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr SetWindowsHookEx(HookType hookType, HookProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool UnhookWindowsHookEx(IntPtr hhk);
    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
    [DllImport("user32.dll")]
    private static extern IntPtr WindowFromPoint(POINT Point);

}
