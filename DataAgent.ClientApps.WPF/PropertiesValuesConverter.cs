using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using DataAgent.API.Monitoring;

namespace DataAgent.ClientApps.WPF;
internal class PropertiesValuesConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var values = "None";
        if (value is IMonitorMessage msg && msg.Properties?.Count > 0)
        {
            values = msg.GetPropertiesValues();
        }
        return values;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
internal class PropertiesValuesConverterWithoutError : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var values = "None";
        if (value is IMonitorMessage msg && !msg.IsError && msg.Properties?.Count > 0)
        {
            values = msg.GetPropertiesValues();
        }
        return values;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}

