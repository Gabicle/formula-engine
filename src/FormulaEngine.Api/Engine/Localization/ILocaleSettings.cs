namespace FormulaEngine.Api.Engine.Localization;

public interface ILocaleSettings
{
    char DecimalSeparator { get; }
    char ArgumentSeparator { get; }
    string LocaleCode { get; }
}
