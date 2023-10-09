using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAgent.API.Monitoring.Intenral;
public interface IMonitorMessageExtendable : IMonitorMessage
{
    void SetPropertyValue(string propertyName, string value);
}
