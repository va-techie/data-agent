using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAgent.API;
public static class Literals
{
    public static class KnownMonitors
    {
        public const string Mouse = "Mouse";
        public const string Keyboard = "Keboard";
    }

    public static class Patterns
    {
        public const string Toggle = "Toggle";
        public const string Text = "Text";
        public const string Invoke = "Invoke";
        public const string Scroll = "Scroll";
        public const string ExpandCollapse = "ExpandCollapse";
    }
    public static class Properties
    {
        public static class Names
        {
            public const string Key = "Key";
            public const string Value = "Value";

            public const string Error = "Error";
        }

        public const string UnsetValue = "Unset";
        public const int UnsetProcessId = -1;
        public const int UnsetPosition = -1;
    }
    public static class EventNames
    {
        public const string MouseLeft = "Mouse Left";
        public const string MouseRight = "Mouse Right";
        public const string KeyDown = "Key Down";


        public const string Exception = "Exception";
    }
}
