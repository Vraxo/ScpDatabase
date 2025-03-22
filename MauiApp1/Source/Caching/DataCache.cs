using System.Diagnostics;
using Newtonsoft.Json;
using Supabase;

namespace ScpDatabase;

public static class DataCache
{
    public static readonly string ImagesFolderPath = Path.Combine(FileSystem.AppDataDirectory, "Images");
    
    private static readonly string supabaseUrl = "https://clxmclkjwvfqpwopwcfe.supabase.co";
    private static readonly string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImNseG1jbGtqd3ZmcXB3b3B3Y2ZlIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MzUzOTYyNjYsImV4cCI6MjA1MDk3MjI2Nn0.8EzuVJ1MByMEQGBLLw5Rj6gKConBBSupeLxCtNCpDEA";
    private static readonly Client supabase = new(supabaseUrl, supabaseKey);
    private static readonly string cacheFileName = "data.json";
    private static readonly HttpClient httpClient = new();
    
    public static async Task LoadAndCacheAllData()
    {
        try
        {
            EnsureImagesFolderExists();

            List<ScpModel> scpData = await FetchScpData();
            List<PersonnelModel> personnelData = await FetchPersonnelData();

            await CacheData(scpData, personnelData);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error during data caching: {ex.Message}");
        }
    }

    private static void EnsureImagesFolderExists()
    {
        if (Directory.Exists(ImagesFolderPath))
        {
            return;
        }

        Directory.CreateDirectory(ImagesFolderPath);
    }

    private static async Task<List<ScpModel>> FetchScpData()
    {
        var response = await supabase.From<ScpModel>().Get();
        return response.Models;
    }

    private static async Task<List<PersonnelModel>> FetchPersonnelData()
    {
        var response = await supabase.From<PersonnelModel>().Get();
        return response.Models;
    }

    private static async Task CacheData(List<ScpModel> scpData, List<PersonnelModel> personnelData)
    {
        CachedData cachedData = new()
        {
            SCPs = scpData,
            Personnel = personnelData
        };

        await SaveCachedData(cachedData);

        await CacheImages(scpData, "SCPs");
        await CacheImages(personnelData, "Personnel");
    }

    private static async Task SaveCachedData(CachedData data)
    {
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        string cacheFilePath = Path.Combine(FileSystem.AppDataDirectory, cacheFileName);
        await File.WriteAllTextAsync(cacheFilePath, json);
    }

    private static async Task CacheImages<T>(List<T> data, string type) where T : IImageModel
    {
        foreach (var item in data)
        {
            await CacheSingleImage(item, type);
        }
    }

    private static async Task CacheSingleImage<T>(T item, string type) where T : IImageModel
    {
        string imageFileName = Path.GetFileName(item.ImageName);
        string localImagePath = Path.Combine(ImagesFolderPath, imageFileName);

        if (File.Exists(localImagePath))
        {
            Debug.WriteLine($"Image already cached: {imageFileName}");
            return;
        }

        string imageUrl = GetImageUrl(type, item.ImageName);

        try
        {
            byte[] imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
            await File.WriteAllBytesAsync(localImagePath, imageBytes);
            Debug.WriteLine($"Downloaded and cached: {imageFileName}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error downloading image '{item.ImageName}' from '{type}': {ex.Message}");
        }
    }

    private static string GetImageUrl(string type, string imageName)
    {
        return $"{supabaseUrl}/storage/v1/object/public/Images/{type}/{imageName}";
    }

    public static async Task<CachedData>? LoadCachedData()
    {
        string cacheFilePath = Path.Combine(FileSystem.AppDataDirectory, cacheFileName);

        if (!File.Exists(cacheFilePath))
        {
            return null;
        }

        try
        {
            string json = await File.ReadAllTextAsync(cacheFilePath);
            return JsonConvert.DeserializeObject<CachedData>(json);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error reading or deserializing cached data: {ex.Message}");
        }

        return null;
    }
}