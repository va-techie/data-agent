using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAgent.API.Monitoring;
public interface IMessagePublisher
{
    string Name
    {
        get;
    }

    string DisplayName
    {
        get;
    }

    bool IsEnabled
    {
        get;
    }

    void Subscribe(Action<IMonitorMessage> callback);

    void Unsubscribe(Action<IMonitorMessage> callback);
}
