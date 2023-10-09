
using DataAgent.API.Monitoring;
using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Threading;

internal class Program
{
    private static void Main(string[] args)
    {
        DataAgent.FrameworkProxy.Framework.Initialize();

        Console.WriteLine("Monitoring Started!");

        SubscribeToMessages();

        Console.ReadLine();
        //string? readText = String.Empty;
        //while (String.Compare(readText, "EXIT", StringComparison.OrdinalIgnoreCase) != 0)
        //{
        //    readText = Console.ReadLine();
        //}

        UnsubscribeToMessages();

        DataAgent.FrameworkProxy.Framework.Shutdown();
    }

    private static void SubscribeToMessages()
    {
        DataAgent.API.Services.Providers.GlobalMessagePublisher.Subscribe(OnMessageRecieved);
    }

    private static void UnsubscribeToMessages()
    {
        DataAgent.API.Services.Providers.GlobalMessagePublisher.Unsubscribe(OnMessageRecieved);
    }

    private static void OnMessageRecieved(IMonitorMessage msg)
    {
        if (msg == null) return;

        string userMessage = $"{msg.TimeStamp} | {msg.ProcessName}:{msg.WindowTitle} | Monitor: {msg.Sender} | Event: {msg.Event} | Mouse: {msg.CursorPosition}";
        Console.WriteLine(userMessage);
        userMessage = $"Input Element : {msg.InputElementType} : {msg.InputElementName} | Focused Element : {msg.FocusedElementType} : {msg.FocusedElementName}";
        Console.WriteLine(userMessage);
        if (msg.Properties.Count > 0)
            Console.WriteLine(msg.GetPropertiesValues());
    }
}