using System.Globalization;

namespace GestionnaireLivres.MAUI.Converters;

public class GenreToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string genre)
        {
            return genre switch
            {
                "Roman" => Color.FromArgb("#3B82F6"),
                "SF" => Color.FromArgb("#8B5CF6"),
                "Fantasy" => Color.FromArgb("#22C55E"),
                "Policier" => Color.FromArgb("#EF4444"),
                _ => Color.FromArgb("#6B7280")
            };
        }
        return Color.FromArgb("#6B7280");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
