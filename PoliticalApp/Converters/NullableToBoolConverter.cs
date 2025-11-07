using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace PoliticalApp.Converters;

/// <summary>
/// Returns true when the bound value is not null, enabling visibility bindings for nullable sources.
/// </summary>
public class NullableToBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value != null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException("NullableToBoolConverter does not support ConvertBack.");
    }
}
