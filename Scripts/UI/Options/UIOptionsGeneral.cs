namespace Planets;

public partial class UIOptionsGeneral : Control
{
    private ResourceOptions Options { get; set; }

    public override void _Ready()
    {
        Options = OptionsManager.Options;

        SetupLanguage();
    }

    private void SetupLanguage()
    {
        OptionButton optionButtonLanguage = GetNode<OptionButton>("Language/Language");
        optionButtonLanguage.Select((int)Options.Language);
    }

    private void _on_language_item_selected(int index)
    {
        string locale = ((Language)index).ToString().Substring(0, 2).ToLower();

        TranslationServer.SetLocale(locale);

        Options.Language = (Language)index;
    }
}

public enum Language
{
    English,
    French,
    Japanese
}
