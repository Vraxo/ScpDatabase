namespace ScpDatabase;

public partial class ItemDetailPage : ContentPage
{
    public ItemDetailPage(DisplayModel item)
    {
        InitializeComponent();

        ItemImage.Source = new FileImageSource { File = item.DisplayImage };

        if (item.Type == "SCP")
        {
            LoadScpDetails(item);
        }
        else if (item.Type == "Personnel")
        {
            LoadPersonnelDetails(item);
        }
    }

    private async void LoadScpDetails(DisplayModel item)
    {
        var cachedData = await DataCache.LoadCachedData();
        var scp = cachedData?.SCPs.FirstOrDefault(s => s.Number == item.DisplayText);

        if (scp != null)
        {
            AddDetailRow("Number", scp.Number);
            AddDetailRow("Class", scp.Class);
            AddDetailRow("Procedures", scp.Procedures);
            AddDetailRow("Description", scp.Description);
            AddDetailRow("Status", scp.Status);
            AddDetailRow("Username", scp.Username);
        }
    }

    private async void LoadPersonnelDetails(DisplayModel item)
    {
        var cachedData = await DataCache.LoadCachedData();
        var personnel = cachedData?.Personnel.FirstOrDefault(m => m.Name == item.DisplayText);

        if (personnel != null)
        {
            AddDetailRow("Name", personnel.Name);
            AddDetailRow("Nationality", personnel.Nationality);
            AddDetailRow("Age", personnel.Age.ToString());
            AddDetailRow("Profession", personnel.Profession);
            AddDetailRow("History", personnel.History);
            AddDetailRow("Department", personnel.Department);
            AddDetailRow("Class", personnel.Class);
            AddDetailRow("Division", personnel.Division);
            AddDetailRow("Clearance Level", personnel.ClearanceLevel.ToString());
            AddDetailRow("Username", personnel.Username);
        }
    }

    private void AddDetailRow(string property, string value)
    {
        int rowCount = ItemDetailsGrid.RowDefinitions.Count;

        // Add a row for the divider
        ItemDetailsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        // Add a property label, spanning 2 columns
        var propertyLabel = new Label
        {
            Text = property,
            FontAttributes = FontAttributes.Bold,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalOptions = LayoutOptions.Center
        };
        ItemDetailsGrid.Add(propertyLabel, 0, rowCount);
        ItemDetailsGrid.SetColumnSpan(propertyLabel, 2);

        // Add a row for the value label
        ItemDetailsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        // Add a value label, spanning 2 columns
        var valueLabel = new Label
        {
            Text = value,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalOptions = LayoutOptions.Center
        };
        ItemDetailsGrid.Add(valueLabel, 0, rowCount + 1);
        ItemDetailsGrid.SetColumnSpan(valueLabel, 2);

        // Add a row for the divider
        ItemDetailsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        // Add a BoxView as a divider, spanning 2 columns
        var divider = new BoxView
        {
            Color = Colors.LightGray,
            HeightRequest = 1,
            HorizontalOptions = LayoutOptions.FillAndExpand,
            Margin = new Thickness(0, 5)
        };
        ItemDetailsGrid.Add(divider, 0, rowCount + 2);
        ItemDetailsGrid.SetColumnSpan(divider, 2);
    }
}