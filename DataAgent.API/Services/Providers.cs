using DataAgent.API.Monitoring;
using DataAgent.API.Monitoring.Intenral;
using DataAgent.API.Services.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAgent.API.Services;

public static class Providers
{
    internal static readonly IPublisherProviderCatalog InternalPublisherProvider = new PublisherProvider();
    public static IPublisherProvider PublisherProvider => InternalPublisherProvider;

    public static void AddPublisherByFramework(string name, IMessagePublisher publisher)
    {
        InternalPublisherProvider.AddPublisher(name, publisher);
    }

    public static void RemovePublisherByFramework(IMessagePublisher publisher)
    {
        InternalPublisherProvider.RemovePublisher(publisher);
    }

    public static IMessagePublisher GlobalMessagePublisher => new Lazy<IMessagePublisher>(() => new GlobalMessagePublisher(PublisherProvider)).Value;

}
