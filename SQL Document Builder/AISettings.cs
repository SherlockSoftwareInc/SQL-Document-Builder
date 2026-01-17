using System.Text.Json;
using System.IO;

namespace SQL_Document_Builder
{
    /// <summary>
    /// Represents AI-related settings for persistence.
    /// </summary>
    public class AISettings
    {
        public string AIEndpoint { get; set; } = string.Empty;
        public string AIModel { get; set; } = string.Empty;
        public string AIApiKey { get; set; } = string.Empty;
        public string AILanguage { get; set; } = "English";

        private static string GetSettingsPath()
        {
            var dataPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "Sherlock Software Inc", "SQLDocBuilder");
            if (!Directory.Exists(dataPath))
                Directory.CreateDirectory(dataPath);
            return Path.Combine(dataPath, "AISettings.json");
        }

        public static AISettings Load()
        {
            var path = GetSettingsPath();
            if (!File.Exists(path))
                return new AISettings();
            try
            {
                var json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<AISettings>(json) ?? new AISettings();
            }
            catch
            {
                return new AISettings();
            }
        }

        public void Save()
        {
            var path = GetSettingsPath();
            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }
    }
}
        