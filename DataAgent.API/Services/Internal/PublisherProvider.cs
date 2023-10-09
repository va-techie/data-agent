using DataAgent.API.Monitoring;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAgent.API.Services.Internal;

internal class PublisherProvider : IPublisherProviderCatalog
{
    private readonly Dictionary<string, IMessagePublisher> providers = new();
    public IMessagePublisher? GetPublisher(string name)
    {
        lock (providers)
        {
            if (providers.ContainsKey(name))
                return providers[name];
            else
                return null;
        }
    }

    public void GetAllPublishers(out IMessagePublisher[] publisher)
    {
        lock (providers)
            publisher = providers.Values.ToArray();
    }

    public void AddPublisher(string name, IMessagePublisher publisher)
    {
        lock (providers)
            providers[name] = publisher;
    }

    public void RemovePublisher(IMessagePublisher publisher)
    {
        lock (providers)
        {
            var item = providers.FirstOrDefault(p => p.Value == publisher);
            providers.Remove(item.Key);
        }
    }
}
