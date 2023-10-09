using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAgent.API.Monitoring;
public interface IMonitorMessage
{
    DateTime TimeStamp
    {
        get;
    }
    string Sender
    {
        get;
    }
    string ProcessName
    {
        get;
    }
    int ProcessId
    {
        get;
    }
    string WindowTitle
    {
        get;
    }

    string Event
    {
        get;
    }

    Point CursorPosition
    {
        get;
    }

    string InputElementType
    {
        get;
    }
    string InputElementName
    {
        get;
    }

    string FocusedElementType
    {
        get;
    }

    string FocusedElementName
    {
        get;
    }

    IReadOnlyDictionary<string, string> Properties
    {
        get;
    }

    string GetPropertiesValues();

    bool IsError
    {
        get;
    }

}

