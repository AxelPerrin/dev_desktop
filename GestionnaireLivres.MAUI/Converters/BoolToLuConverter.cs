using System.Globalization;

namespace GestionnaireLivres.MAUI.Converters;

public class BoolToLuConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool lu)
            return lu ? "✓ Lu" : "✗ Non lu";
        return "✗ Non lu";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
