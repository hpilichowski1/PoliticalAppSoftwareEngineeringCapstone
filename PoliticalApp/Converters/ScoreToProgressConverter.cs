using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace PoliticalApp.Converters;

/// <summary>
/// Converts a percentage value (0-100+) to a normalized progress value (0-1).
/// </summary>
public class ScoreToProgressConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return 0d;
        }

        double score = value switch
        {
            double d => d,
            float f => f,
            int i => i,
            _ => 0d
        };

        return Math.Clamp(score / 100d, 0d, 1d);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}
