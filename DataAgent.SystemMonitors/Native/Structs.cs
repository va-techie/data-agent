using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DataAgent.Monitors.System.Native;


[StructLayout(LayoutKind.Sequential)]
struct POINT
{
    public int X;
    public int Y;
}
