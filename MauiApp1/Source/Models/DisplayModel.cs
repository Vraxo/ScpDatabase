using System.Globalization;

namespace ScpDatabase;

public class DisplayModel
{
    public string DisplayImage { get; set; } = "";
    public string DisplayText { get; set; } = "";
    public string Type { get; set; } = "";
    public ScpModel Scp { get; set; } = new();
    public PersonnelModel Mtf { get; set; } = new();
}

public class NullToFalseConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is not null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}