namespace TheBrowser.Models;

public class FingerprintProfile
{
    public string UserAgent { get; set; } = "";
    public string Language { get; set; } = "";
    public int ScreenWidth { get; set; }
    public int ScreenHeight { get; set; }
    public int ScreenAvailableHeight { get; set; }
    public int ScreenAvailableWidth { get; set; }
    public int ColorDepth { get; set; }
    public string Platform { get; set; } = "";
    public int DeviceMemory { get; set; }
    public int HardwareConcurrency { get; set; }
    public int MaxTouchPoints { get; set; }
    public string DoNotTrack { get; set; } = "1";
    public string WebGLVendor { get; set; } = "";
    public string WebGLRenderer { get; set; } = "";
    public bool LocalStorage { get; set; } = true;
    public bool SessionStorage { get; set; } = true;
    public bool IndexedDB { get; set; } = true;
    public string Timezone { get; set; } = "";
    public string Locale { get; set; } = "en-US";
    public List<string> Plugins { get; set; } = new();
    public string AudioFingerprint { get; set; } = string.Empty;
    public string CanvasFingerprint { get; set; } = "";
}

