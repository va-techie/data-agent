using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataAgent.API.Services;

namespace DataAgent.API.Monitoring.Intenral;
public sealed class GlobalMessagePublisher : MessagePublisher
{
    readonly IPublisherProvider provider;
    public GlobalMessagePublisher(IPublisherProvider provider)
    {
        this.provider = provider;
        name = "Global Messages";
        Initialize();
    }

    void Initialize()
    {
        provider.GetAllPublishers(out IMessagePublisher[] pubs);
        if (pubs != null && pubs.Length > 0)
        {
            foreach (var pub in pubs)
                pub.Subscribe(MessageReceived);
        }
    }

    void Shutdown()
    {
        isEnabled = false;
        provider.GetAllPublishers(out IMessagePublisher[] pubs);
        if (pubs != null && pubs.Length > 0)
        {
            foreach (var pub in pubs)
                pub.Unsubscribe(MessageReceived);
        }
    }

    private void MessageReceived(IMonitorMessage message)
    {
        this.Publish(message);
    }

    protected override void Publish(IMonitorMessage message)
    {
        publishQueue.Enqueue(message);
        ThreadPool.QueueUserWorkItem((state) => PublishFromQueue());
    }
}
