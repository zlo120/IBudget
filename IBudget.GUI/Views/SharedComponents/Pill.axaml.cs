using Avalonia;
using Avalonia.Controls;

namespace IBudget.GUI.SharedComponents;

public partial class Pill : UserControl
{
    public static readonly StyledProperty<string> PillTextProperty = AvaloniaProperty.Register<Pill, string>(nameof(PillText));
    public string PillText
    {
        get => GetValue(PillTextProperty);
        set => SetValue(PillTextProperty, value);
    }

    public static readonly StyledProperty<string> VariantProperty = AvaloniaProperty.Register<Pill, string>(nameof(Variant));
    public string Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    public Pill()
    {
        InitializeComponent();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == VariantProperty)
            UpdateClasses();

        if (change.Property == PillTextProperty)
        {
            var textBlock = this.FindControl<TextBlock>("Text");
            if (textBlock != null)
                textBlock.Text = PillText;
        }
    }

    private void UpdateClasses()
    {
        var text = this.FindControl<TextBlock>("Text");
        var border = this.FindControl<Border>("Border");

        if (text != null)
        {
            text.Classes.Clear();
            if (!string.IsNullOrEmpty(Variant))
                text.Classes.Add(Variant);
        }

        if (border != null)
        {
            border.Classes.Clear();
            if (!string.IsNullOrEmpty(Variant))
                border.Classes.Add(Variant);
        }
    }
}