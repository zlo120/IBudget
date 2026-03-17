using Avalonia;
using Avalonia.Controls;

namespace IBudget.GUI.SharedComponents;

public partial class Card : UserControl
{
    public static readonly StyledProperty<string> LabelProperty = AvaloniaProperty.Register<Card, string>(nameof(Label));
    public string Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public static readonly StyledProperty<string> ValueProperty = AvaloniaProperty.Register<Card, string>(nameof(Value));
    public string Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static readonly StyledProperty<string> HelperTextProperty = AvaloniaProperty.Register<Card, string>(nameof(HelperText));
    public string HelperText
    {
        get => GetValue(HelperTextProperty);
        set => SetValue(HelperTextProperty, value);
    }

    public static readonly StyledProperty<string> VariantProperty = AvaloniaProperty.Register<Card, string>(nameof(Variant));
    public string Variant
    {
        get => GetValue(VariantProperty);
        set
        {
            SetValue(VariantProperty, value);
            UpdateClasses();
        }
    }

    public Card()
    {
        InitializeComponent();
        DataContext = this;
    }

    private void UpdateClasses()
    {
        var border = this.FindControl<Border>("CardBorder");
        var valueText = this.FindControl<TextBlock>("ValueText");

        if (border != null)
        {
            border.Classes.Clear();
            if (!string.IsNullOrEmpty(Variant))
                border.Classes.Add(Variant);
        }

        if (valueText != null)
        {
            valueText.Classes.Clear();
            if (!string.IsNullOrEmpty(Variant))
                valueText.Classes.Add(Variant);
        }
    }
}