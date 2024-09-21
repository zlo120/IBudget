using Avalonia.Controls;
using System.Linq;

namespace IBudget.GUI.Views.UploadCsv;

public partial class TagDataStepPageView : UserControl
{
    public TagDataStepPageView()
    {
        InitializeComponent();
        var tagsSource = new string[]
        {
            "food", "petrol", "entertainment", "bills", "work", "other"
        }.OrderBy(tag => tag);

        entriesTags.ItemsSource = tagsSource;
        rulesTags.ItemsSource = tagsSource;
    }
}