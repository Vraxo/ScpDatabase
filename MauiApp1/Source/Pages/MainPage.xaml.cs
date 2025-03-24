using System.Collections.ObjectModel;

namespace ScpDatabase;

public partial class MainPage : ContentPage
{
    public ObservableCollection<DisplayModel> DisplayItems { get; set; } = [];

    private List<string> allFilterOptions = ["SCPs", "Researchers", "MTFs", "Agents", "Ethicists", "None"];
    private readonly Dictionary<string, bool> filterStates = [];

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

    private async void OnFilterOptionChanged(object? sender, CheckedChangedEventArgs e)
    {
        if (sender is not CheckBox checkbox) return;

        var option = (string)checkbox.BindingContext;
        filterStates[option] = e.Value;

        await ApplyFilters();
    }

    private async Task ApplyFilters()
    {
        CachedData? cachedData = await DataCache.LoadCachedData();

        if (cachedData is null)
        {
            await DisplayAlert("No Data", "No cached data found. Please sync data.", "OK");
            return;
        }

        List<DisplayModel> filteredItems = [];

        AddScpItems(cachedData, filteredItems);
        AddPersonnelItems(cachedData, filteredItems);

        UpdateDisplayItems(filteredItems);
    }

    private void AddScpItems(CachedData cachedData, List<DisplayModel> filteredItems)
    {
        if (filterStates["SCPs"] && cachedData.SCPs != null)
        {
            filteredItems.AddRange(cachedData.SCPs
                .Where(scp => scp != null)
                .Select(CreateDisplayModelFromScp));
        }
    }

    private void AddPersonnelItems(CachedData cachedData, List<DisplayModel> filteredItems)
    {
        if (cachedData.Personnel == null)
        {
            return;
        }

        List<(string FilterKey, string? Department)> personnelFilters =
        [
            ("Researchers", "Researcher"),
            ("MTFs", "MTF"),
            ("Agents", "Agent"),
            ("Ethicists", "Ethicist"),
            ("None", null)
        ];

        foreach (var (FilterKey, Department) in personnelFilters)
        {
            if (!filterStates[FilterKey])
            {
                continue;
            }

            IEnumerable<PersonnelModel> personnel = Department == null
                ? cachedData.Personnel.Where(p => string.IsNullOrEmpty(p.Department))
                : cachedData.Personnel.Where(p => p.Department == Department);

            filteredItems.AddRange(personnel.Select(CreateDisplayModelFromPersonnel));
        }
    }

    private DisplayModel CreateDisplayModelFromScp(ScpModel scp)
    {
        return new()
        {
            DisplayImage = GetImagePath(scp.ImageName),
            DisplayText = scp.Number,
            Type = "SCP"
        };
    }

    private DisplayModel CreateDisplayModelFromPersonnel(PersonnelModel personnel)
    {
        return new()
        {
            DisplayImage = GetImagePath(personnel.ImageName),
            DisplayText = personnel.Name,
            Type = "Personnel"
        };
    }

    private static string GetImagePath(string imageName)
    {
        return Path.Combine(DataCache.ImagesFolderPath, Path.GetFileName(imageName));
    }

    private void UpdateDisplayItems(List<DisplayModel> filteredItems)
    {
        Dispatcher.Dispatch(() =>
        {
            DisplayItems.Clear();

            foreach (DisplayModel? item in filteredItems.OrderBy(i => i.DisplayText))
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
#if (ANDROID || WINDOWS) && DEBUG
            // Only open folder in debug builds for Android/Windows
            Process.Start(new ProcessStartInfo
            {
                FileName = DataCache.ImagesFolderPath,
                UseShellExecute = true,
                Verb = "open"
            });
#else
                // For release builds or other platforms
                await DisplayAlert("Cache Location",
                    $"Files cached at: {DataCache.ImagesFolderPath}",
                    "OK");
#endif
            }
            else
            {
                await DisplayAlert("Error", "Cache folder not found.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to access cache: {ex.Message}", "OK");
        }
    }

    private async void OnItemTapped(object sender, EventArgs e)
    {
        var senderElement = (Element)sender;
        var item = (DisplayModel)senderElement.BindingContext;

        await Navigation.PushAsync(new ItemDetailPage(item));
    }
}