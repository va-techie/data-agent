using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAgent.API.Monitoring.Intenral;
public interface IMessagePublisherHookable : IMessagePublisher
{
    void AddHook(Predicate<IMonitorMessage> hook);

    void RemoveHook(Predicate<IMonitorMessage> hook);
}
