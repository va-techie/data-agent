using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAgent.API.Monitoring.Intenral.Monitors;
public interface IMonitorMouse : IMessagePublisherHookable
{
    void GetCursorPosition(out int x, out int y);
}
