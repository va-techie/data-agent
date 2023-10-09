using DataAgent.API.Monitoring;
using DataAgent.API.Monitoring.Intenral.Monitors;
using System;
using System.Security.Cryptography.X509Certificates;

internal class Program
{
    private static void Main(string[] args)
    {
        DataAgent.FrameworkProxy.Framework.Initialize();

        Console.WriteLine("Monitoring Started!");

        SubscribeToMessages();

        Console.ReadLine();

        UnsubscribeToMessages();

        DataAgent.FrameworkProxy.Framework.Shutdown();
    }

    private static void SubscribeToMessages()
    {
        var mouseMonitor = DataAgent.API.Services.Providers.PublisherProvider.GetPublisher(DataAgent.API.Literals.KnownMonitors.Mouse);
        mouseMonitor?.Subscribe(OnMessageRecieved);

        DataAgent.API.Services.Providers.PublisherProvider.GetAllPublishers(out var publishers);
        foreach (var publisher in publishers)
        {
            if (publisher != mouseMonitor && publisher is IMessagePublishControl publishControl)
                publishControl.IsEnabled = false;
        }
    }

    private static void UnsubscribeToMessages()
    {
        var mouseMonitor = DataAgent.API.Services.Providers.PublisherProvider.GetPublisher(DataAgent.API.Literals.KnownMonitors.Mouse);
        mouseMonitor?.Unsubscribe(OnMessageRecieved);
    }

    private static void OnMessageRecieved(IMonitorMessage msg)
    {
        if (msg == null) return;

        var userMessage = $"{msg.TimeStamp} | {msg.ProcessName}:{msg.WindowTitle} | Event: {msg.Event} | Mouse: {msg.CursorPosition}";
        Console.WriteLine(userMessage);
        userMessage = $"Input Element : {msg.InputElementType} : {msg.InputElementName}";
        Console.WriteLine(userMessage);
    }
}