namespace ScpDatabase;

public partial class ItemDetailPage : ContentPage
{
    public ItemDetailPage(DisplayModel item)
    {
        InitializeComponent();

        ItemImage.Source = new FileImageSource 
        { 
            File = item.DisplayImage 
        };

        LoadDetails(item);
    }

    private async void LoadDetails(DisplayModel item)
    {
        if (item.Type == "SCP")
        {
            await LoadScpDetails(item.DisplayText);
        }
        else if (item.Type == "Personnel")
        {
            await LoadPersonnelDetails(item.DisplayText);
        }
    }

    private async Task LoadScpDetails(string displayText)
    {
        var model = await GetCachedModel<ScpModel>(
            cd => cd.SCPs,
            s => s.Number == displayText
        );

        if (model is not null)
        {
            AddDetails(ItemDetailPage.GetScpDetails(model));
        }
    }

    private async Task LoadPersonnelDetails(string displayText)
    {
        var model = await GetCachedModel<PersonnelModel>(
            cd => cd.Personnel,
            p => p.Name == displayText
        );

        if (model != null)
        {
            AddDetails(ItemDetailPage.GetPersonnelDetails(model));
        }
    }

    private static async Task<TModel?> GetCachedModel<TModel>(Func<CachedData, IEnumerable<TModel>> collectionSelector, Func<TModel, bool> predicate)
    {
        CachedData? cachedData = await DataCache.LoadCachedData();
        return cachedData != null ? collectionSelector(cachedData).FirstOrDefault(predicate) : default;
    }

    private static IEnumerable<(string Label, string Value)> GetScpDetails(ScpModel scp)
    {
        return
        [
            ("Number", scp.Number),
            ("Class", scp.Class),
            ("Procedures", scp.Procedures),
            ("Description", scp.Description),
            ("Status", scp.Status),
            ("Username", scp.Username)
        ];
    }

    private static IEnumerable<(string Label, string Value)> GetPersonnelDetails(PersonnelModel personnel)
    {
        return 
        [
            ("Name", personnel.Name),
            ("Nationality", personnel.Nationality),
            ("Age", personnel.Age.ToString()),
            ("Profession", personnel.Profession),
            ("History", personnel.History),
            ("Department", personnel.Department),
            ("Class", personnel.Class),
            ("Division", personnel.Division),
            ("Clearance Level", personnel.ClearanceLevel.ToString()),
            ("Username", personnel.Username)
        ];
    }

    private void AddDetails(IEnumerable<(string Label, string Value)> details)
    {
        foreach ((string Label, string Value) detail in details)
        {
            AddDetailRow(detail.Label, detail.Value);
        }
    }

    private void AddDetailRow(string propertyName, string propertyValue)
    {
        int currentRow = ItemDetailsGrid.RowDefinitions.Count;

        ItemDetailsGrid.RowDefinitions.Add(new()
        {
            Height = GridLength.Auto
        });

        AddLabel(propertyName, currentRow, FontAttributes.Bold);

        ItemDetailsGrid.RowDefinitions.Add(new()
        {
            Height = GridLength.Auto
        });

        AddLabel(propertyValue, currentRow + 1);

        ItemDetailsGrid.RowDefinitions.Add(new()
        {
            Height = GridLength.Auto
        });

        AddDivider(currentRow + 2);
    }

    private void AddLabel(string text, int row, FontAttributes fontAttributes = FontAttributes.None)
    {
        Label label = new()
        {
            Text = text,
            FontAttributes = fontAttributes,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalOptions = LayoutOptions.Center
        };
        ItemDetailsGrid.Add(label, 0, row);
        ItemDetailsGrid.SetColumnSpan(label, 2);
    }

    private void AddDivider(int row)
    {
        BoxView divider = new()
        {
            Color = Colors.LightGray,
            HeightRequest = 1,
            HorizontalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 5)
        };
        ItemDetailsGrid.Add(divider, 0, row);
        ItemDetailsGrid.SetColumnSpan(divider, 2);
    }
}