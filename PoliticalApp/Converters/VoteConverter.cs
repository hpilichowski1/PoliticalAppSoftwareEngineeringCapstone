using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using PoliticalApp.Models;

namespace PoliticalApp.Converters
{
    public class VoteToUpColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is VoteType vote && vote == VoteType.Up)
                return Colors.Green;

            return Colors.LightGray;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class VoteToDownColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is VoteType vote && vote == VoteType.Down)
                return Color.FromRgb(152, 10, 10); // RED

            return Colors.LightGray;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
