using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using DataAgent.API.Monitoring;
using Microsoft.UI.Dispatching;
using WinUIEx.Messaging;

namespace DataAgent.ClientApps.WinUI.ViewModels;

public partial class MessagesViewModel : ObservableRecipient
{
    public MessagesViewModel()
    {
        Messages = new ReadOnlyObservableCollection<IMonitorMessage>(messageLog);
        ClearMessagesCommand = new ClearCommand(ClearMessages);
        SubscribeToMessages();
    }
    private bool loggingEnabled = true;

    public bool LoggingEnabled
    {
        get => loggingEnabled;
        set
        {
            loggingEnabled = value;
            OnPropertyChanged(nameof(LoggingEnabled));
        }
    }

    private bool logErrors = false;

    public bool LogErrors
    {
        get => logErrors;
        set
        {
            logErrors = value;
            OnPropertyChanged(nameof(LogErrors));
        }
    }

    public ICommand ClearMessagesCommand
    {
        get; private set;
    }

    private readonly ObservableCollection<IMonitorMessage> messageLog = new();

    public ReadOnlyObservableCollection<IMonitorMessage> Messages
    {
        get; private set;
    }
    public DispatcherQueue DispatcherQueue
    {
        get;
        internal set;
    }

    private void ClearMessages(object? obj)
    {
        messageLog.Clear();
    }
    internal void Unsubscribe()
    {
        LoggingEnabled = false;
        UnsubscribeToMessages();
    }

    private void SubscribeToMessages()
    {
        DataAgent.API.Services.Providers.PublisherProvider.GetAllPublishers(out var publishers);

        DataAgent.API.Services.Providers.GlobalMessagePublisher.Subscribe(OnMessageRecieved);
    }

    private void UnsubscribeToMessages()
    {
        DataAgent.API.Services.Providers.GlobalMessagePublisher.Unsubscribe(OnMessageRecieved);
    }
    private void OnMessageRecieved(IMonitorMessage msg)
    {
        if (msg == null) return;

        if (loggingEnabled)
        {
            DispatcherQueue.TryEnqueue(() => LogMessage(msg));
        }
    }

    private void LogMessage(IMonitorMessage msg)
    {
        if (loggingEnabled)
        {
            if (!logErrors)
            {
                if (!msg.IsError)
                    messageLog.Add(msg);
            }
            else
                messageLog.Add(msg);
        }

    }
    class ClearCommand : ICommand
    {
        readonly Action<object?> execute;
        public ClearCommand(Action<object?> onExecute)
        {
            execute = onExecute;
        }
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) => execute?.Invoke(parameter);
    }
}
