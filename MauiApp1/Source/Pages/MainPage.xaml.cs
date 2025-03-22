using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ScpDatabase;

public partial class MainPage : ContentPage
{
    public ObservableCollection<DisplayModel> DisplayItems { get; set; } = [];

    private List<string> allFilterOptions = ["SCPs", "Researchers", "MTFs", "Agents", "Ethicists", "None"];
    private Dictionary<string, bool> filterStates = [];

    public MainPage()
    {
        InitializeComponent();
        BindingContext = this;
        InitializeFilters();
        LoadData();
    }

    private void InitializeFilters()
    {
        FilterOptions.Children.Clear();

        foreach (string option in allFilterOptions)
        {
            filterStates[option] = true;

            CheckBox checkbox = new()
            {
                IsChecked = true,
                BindingContext = option
            };
            checkbox.CheckedChanged += OnFilterOptionChanged;

            Label label = new()
            {
                Text = option,
                VerticalOptions = LayoutOptions.Center
            };

            TapGestureRecognizer tapRecognizer = new();
            tapRecognizer.Tapped += (s, e) => checkbox.IsChecked = !checkbox.IsChecked;
            label.GestureRecognizers.Add(tapRecognizer);

            HorizontalStackLayout layout = new()
            {
                Spacing = 10,
                Children = { checkbox, label }
            };

            FilterOptions.Children.Add(layout);
        }
    }

    private async void OnFilterOptionChanged(object sender, CheckedChangedEventArgs e)
    {
        var checkbox = (CheckBox)sender;
        var option = (string)checkbox.BindingContext;
        filterStates[option] = e.Value;

        await ApplyFilters();
    }

    private async Task ApplyFilters()
    {
        CachedData cachedData = await DataCache.LoadCachedData();

        if (cachedData is null)
        {
            await DisplayAlert("No Data", "No cached data found. Please sync data.", "OK");
            return;
        }

        List<DisplayModel> filteredItems = [];

        if (filterStates["SCPs"] && cachedData.SCPs != null)
        {
            filteredItems.AddRange(cachedData.SCPs.Where(scp => scp != null).Select(scp => new DisplayModel
            {
                DisplayImage = Path.Combine(DataCache.ImagesFolderPath, Path.GetFileName(scp.ImageName)),
                DisplayText = scp.Number,
                Type = "SCP"
            }));
        }

        if (cachedData.Personnel != null)
        {
            if (filterStates["Researchers"])
            {
                filteredItems.AddRange(cachedData.Personnel.Where(p => p.Department == "Researcher").Select(p => new DisplayModel
                {
                    DisplayImage = Path.Combine(DataCache.ImagesFolderPath, Path.GetFileName(p.ImageName)),
                    DisplayText = p.Name,
                    Type = "Personnel"
                }));
            }

            if (filterStates["MTFs"])
            {
                filteredItems.AddRange(cachedData.Personnel.Where(p => p.Department == "MTF").Select(p => new DisplayModel
                {
                    DisplayImage = Path.Combine(DataCache.ImagesFolderPath, Path.GetFileName(p.ImageName)),
                    DisplayText = p.Name,
                    Type = "Personnel"
                }));
            }

            if (filterStates["Agents"])
            {
                filteredItems.AddRange(cachedData.Personnel.Where(p => p.Department == "Agent").Select(p => new DisplayModel
                {
                    DisplayImage = Path.Combine(DataCache.ImagesFolderPath, Path.GetFileName(p.ImageName)),
                    DisplayText = p.Name,
                    Type = "Personnel"
                }));
            }

            if (filterStates["Ethicists"])
            {
                filteredItems.AddRange(cachedData.Personnel.Where(p => p.Department == "Ethicist").Select(p => new DisplayModel
                {
                    DisplayImage = Path.Combine(DataCache.ImagesFolderPath, Path.GetFileName(p.ImageName)),
                    DisplayText = p.Name,
                    Type = "Personnel"
                }));
            }

            if (filterStates["None"])
            {
                filteredItems.AddRange(cachedData.Personnel.Where(p => string.IsNullOrEmpty(p.Department)).Select(p => new DisplayModel
                {
                    DisplayImage = Path.Combine(DataCache.ImagesFolderPath, Path.GetFileName(p.ImageName)),
                    DisplayText = p.Name,
                    Type = "Personnel"
                }));
            }
        }

        // Update the CollectionView
        Device.BeginInvokeOnMainThread(() =>
        {
            DisplayItems.Clear();
            foreach (var item in filteredItems)
            {
                DisplayItems.Add(item);
            }
        });
    }

    private async void LoadData()
    {
        await ApplyFilters();
    }

    private void OnDropdownTapped(object sender, EventArgs e)
    {
        FilterOptions.IsVisible = !FilterOptions.IsVisible;
    }

    private async void OnSyncButtonClicked(object sender, EventArgs e)
    {
        await DataCache.LoadAndCacheAllData();

        await DisplayAlert("Sync Complete", "Data has been synced successfully!", "OK");

        await ApplyFilters();

        try
        {
            if (Directory.Exists(DataCache.ImagesFolderPath))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = DataCache.ImagesFolderPath,
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
            else
            {
                await DisplayAlert("Error", "Cache folder not found.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to open cache folder: {ex.Message}", "OK");
        }
    }

    private async void OnItemTapped(object sender, EventArgs e)
    {
        var senderElement = (Element)sender;
        var item = (DisplayModel)senderElement.BindingContext;

        await Navigation.PushAsync(new ItemDetailPage(item));
    }
}