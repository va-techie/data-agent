using DataAgent.API.Monitoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAgent.API.Services.Internal;

public interface IPublisherProviderCatalog : IPublisherProvider
{
    void AddPublisher(string name, IMessagePublisher publisher);
    void RemovePublisher(IMessagePublisher publisher);
}
