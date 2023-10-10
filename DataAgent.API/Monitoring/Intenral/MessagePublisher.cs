using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataAgent.API.Monitoring.Intenral;
public abstract class MessagePublisher : IMessagePublisherHookable, IMessagePublishControl, INotifyPropertyChanged
{
    public MessagePublisher()
    {
    }

    private readonly List<Action<IMonitorMessage>> subscribers = new();
    private readonly List<Predicate<IMonitorMessage>> hooks = new();
    protected string name = Literals.Properties.UnsetValue;
    protected string displayName = Literals.Properties.UnsetValue;
    protected bool isEnabled = true;
    protected ConcurrentQueue<IMonitorMessage> publishQueue = new();
    protected int monitorProcessId = Literals.Properties.UnsetProcessId;
    protected DateTime lastHandledEventTimeStamp = DateTime.Now;
    private TimeSpan minWaitForNextEvent = TimeSpan.FromMilliseconds(200);

    protected TimeSpan MinWaitForNextEvent
    {
        get => minWaitForNextEvent;
        set
        {
            minWaitForNextEvent = value;
            RaisePropertyChanged(nameof(MinWaitForNextEvent));
            RaisePropertyChanged(nameof(WaitInBetweenEvents));
        }
    }

    public bool IsEnabled
    {
        get => isEnabled;
        set
        {
            isEnabled = value;
            RaisePropertyChanged(nameof(IsEnabled));
        }
    }
    public double WaitInBetweenEvents
    {
        get => minWaitForNextEvent.TotalMilliseconds;
        set => MinWaitForNextEvent = TimeSpan.FromMilliseconds(value);
    }
    public string Name => name;

    public string DisplayName => displayName;

    public void Subscribe(Action<IMonitorMessage> callback)
    {
        lock (subscribers)
        {
            if (!subscribers.Contains(callback))
                subscribers.Add(callback);
        }
    }

    public void Unsubscribe(Action<IMonitorMessage> callback)
    {
        lock (subscribers)
            subscribers.Remove(callback);
    }

    public void AddHook(Predicate<IMonitorMessage> hook)
    {
        lock (hooks)
        {
            if (!hooks.Contains(hook))
                hooks.Add(hook);
        }
    }

    public void RemoveHook(Predicate<IMonitorMessage> hook)
    {
        lock (hooks)
            hooks.Remove(hook);
    }

    protected virtual void Publish(IMonitorMessage message)
    {
        if (!isEnabled) return;

        var continuePublish = true;

        lock (hooks)
        {
            if (hooks.Count > 0)
            {
                Parallel.ForEach(hooks, hook =>
                {
                    if (!hook(message))
                    {
                        continuePublish = false;
                    }
                });
            }
        }

        if (continuePublish)
        {
            publishQueue.Enqueue(message);
            ThreadPool.QueueUserWorkItem((state) => PublishFromQueue());
        }
    }

    protected virtual void PublishNoHook(IMonitorMessage message)
    {
        if (!isEnabled) return;

        publishQueue.Enqueue(message);
        ThreadPool.QueueUserWorkItem((state) => PublishFromQueue());
    }

    protected void PublishFromQueue()
    {
        while (publishQueue.TryDequeue(out var message))
        {
            if (isEnabled)
            {
                lock (subscribers)
                    Parallel.ForEach(subscribers, subscriber =>
                    {
                        try
                        {
                            subscriber(message);
                        }
                        catch (Exception ex) { PublishException(ex); }
                    });
            }
        }
    }

    protected void PublishException(Exception ex)
    {
        var msg = new MonitorMessage(DateTime.Now, this.Name) { Event = Literals.EventNames.Exception, IsError = true };
        msg.SetPropertyValue(Literals.Properties.Names.Error, ex.Message);
        lock (subscribers)
            Parallel.ForEach(subscribers, subscriber => subscriber(msg));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void RaisePropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
