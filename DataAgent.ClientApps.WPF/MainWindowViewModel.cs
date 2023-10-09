using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using DataAgent.API.Monitoring;

namespace DataAgent.ClientApps.WPF;
public class MainWindowViewModel : INotifyPropertyChanged
{
    public MainWindowViewModel()
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
    public IList<IMessagePublisher> Monitors
    {
        get; private set;
    }

    public ReadOnlyObservableCollection<IMonitorMessage> Messages
    {
        get; private set;
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
        Monitors = publishers.ToList();

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
            Application.Current.Dispatcher.BeginInvoke(() => LogMessage(msg), DispatcherPriority.Normal);
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

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
