using System.Globalization;

namespace FormulaEngine.Api.Engine.Localization;

public class CultureLocaleSettings : ILocaleSettings
{
    public char DecimalSeparator => CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
    public char ArgumentSeparator => DecimalSeparator == ',' ? ';' : ',';
    public string LocaleCode => CultureInfo.CurrentCulture.Name;
}
