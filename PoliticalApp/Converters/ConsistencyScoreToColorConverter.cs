using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace PoliticalApp.Converters;

/// <summary>
/// Maps a consistency score percentage to a semantic color (success/warning/error).
/// </summary>
public class ConsistencyScoreToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var score = 0.0;

        if (value is double doubleValue)
        {
            score = doubleValue;
        }
        else if (value is float floatValue)
        {
            score = floatValue;
        }
        else if (value is int intValue)
        {
            score = intValue;
        }

        var resources = Application.Current?.Resources;

        Color success = TryGetColor(resources, "SuccessColor", Colors.Green);
        Color warning = TryGetColor(resources, "WarningColor", Colors.Orange);
        Color error = TryGetColor(resources, "ErrorColor", Colors.Red);

        if (score >= 80)
        {
            return success;
        }

        if (score >= 60)
        {
            return warning;
        }

        return error;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();

    private static Color TryGetColor(ResourceDictionary? resources, string key, Color fallback)
    {
        if (resources is not null && resources.TryGetValue(key, out var resourceValue) && resourceValue is Color color)
        {
            return color;
        }

        return fallback;
    }
}
