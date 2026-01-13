using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TheBrowser.Models;

namespace TheBrowser.Utils;

public class FingerprintGenerator
{
    private readonly Random _random = new Random();

    /// <summary>
    /// Creates a new, combined fingerprint profile from JSON part files.
    /// </summary>
    /// <returns>A fully configured FingerprintProfile object.</returns>
    /// <exception cref="FileNotFoundException">Thrown when a required JSON file is not found.</exception>
    public FingerprintProfile Generate()
    {
        // Randomly chooses between set1 and set2
        string set = _random.Next(2) == 0 ? "set1" : "set2";
        JObject combinedProfile = new JObject();

        // Combines 5 JSON parts into one profile
        for (int i = 1; i <= 5; i++)
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filename = Path.Combine(documentsPath, $"{set}_teil{i}.json");
            if (!File.Exists(filename))
            {
                // Instead of a MessageBox, an exception is thrown that can be handled in the bot
                throw new FileNotFoundException($"Required fingerprint file not found: {filename}");
            }

            string json = File.ReadAllText(filename);
            JArray profiles = JArray.Parse(json);

            if (profiles.Count == 0) continue;

            // Selects a random profile from the respective part file
            int index = _random.Next(profiles.Count);
            JObject selectedPart = (JObject)profiles[index];

            // Adds the properties to the combined profile
            foreach (var prop in selectedPart.Properties())
            {
                combinedProfile[prop.Name] = prop.Value;
            }
        }

        // Simulates a Canvas fingerprint based on the combined data
        string canvasInput = combinedProfile.ToString(Newtonsoft.Json.Formatting.None);
        combinedProfile["CanvasFingerprint"] = GenerateCanvasFingerprint(canvasInput);

        // Converts the JObject into a strongly typed FingerprintProfile object
        // PropertyNameCaseInsensitive ensures that e.g. "userAgent" is mapped to "UserAgent".
        var serializer = new JsonSerializer { ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver { NamingStrategy = new Newtonsoft.Json.Serialization.CamelCaseNamingStrategy() } };
        return combinedProfile.ToObject<FingerprintProfile>(serializer) ?? new FingerprintProfile();
    }

    /// <summary>
    /// Generates a SHA1 hash as a Canvas fingerprint.
    /// </summary>
    private string GenerateCanvasFingerprint(string input)
    {
        using (var sha1 = SHA1.Create())
        {
            var hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }

    public FingerprintProfile GenerateWithGeo(GeoLocation location)
    {
        var profile = Generate(); // Uses existing logic with JSON files

        profile.Language = location.LangCode;
        profile.Timezone = location.Timezone;
        profile.Locale = location.Locale;

        return profile;
    }
}

