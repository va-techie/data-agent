using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAgent.API.Monitoring.Intenral;
public class MonitorMessage : IMonitorMessageExtendable, INotifyPropertyChanged
{
    protected Dictionary<string, string> properties = new();
    public MonitorMessage(DateTime timestamp, string sender)
    {
        TimeStamp = timestamp;
        Sender = sender;
        CursorPosition = new Point(Literals.Properties.UnsetPosition, Literals.Properties.UnsetPosition);
        ProcessId = Literals.Properties.UnsetProcessId;

        InputElementType = Literals.Properties.UnsetValue;
        InputElementName = Literals.Properties.UnsetValue;
        FocusedElementType = Literals.Properties.UnsetValue;
        FocusedElementName = Literals.Properties.UnsetValue;
        ProcessName = Literals.Properties.UnsetValue;
        WindowTitle = Literals.Properties.UnsetValue;
        Event = Literals.Properties.UnsetValue;
        IsError = false;
    }
    public string InputElementType
    {
        get; set;
    }

    public string InputElementName
    {
        get; set;
    }

    public string FocusedElementType
    {
        get; set;
    }

    public string FocusedElementName
    {
        get; set;
    }

    public DateTime TimeStamp
    {
        get; protected set;
    }

    public string Sender
    {
        get; protected set;
    }

    public string ProcessName
    {
        get; set;
    }

    public int ProcessId
    {
        get; set;
    }

    public string WindowTitle
    {
        get; set;
    }

    public string Event
    {
        get; set;
    }

    public Point CursorPosition
    {
        get; set;
    }

    public IReadOnlyDictionary<string, string> Properties => properties;

    public event PropertyChangedEventHandler? PropertyChanged;

    public virtual void SetPropertyValue(string propertyName, string? value)
    {
        properties[propertyName] = value == null ? Literals.Properties.UnsetValue : value;
    }

    public string GetPropertiesValues()
    {
        return properties.Count > 0 ?
            string.Join(", ", properties.Select(p => $"{p.Key}:{p.Value}")) :
            string.Empty;
    }

    public bool IsVerbose => properties?.Count > 0;

    public bool IsError
    {
        get; set;
    }
}
