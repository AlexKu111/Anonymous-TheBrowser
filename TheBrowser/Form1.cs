using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Text.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using TheBrowser.Models;
using TheBrowser.Services;
using TheBrowser.Utils;

namespace TheBrowser
{
    public partial class Form1 : Form
    {
        private Process? _torProcess;
        private IWebDriver? _browserDriver;
        private bool _isConnected = false;
        private readonly TorManager _torManager;
        private readonly FingerprintGenerator _fingerprintGenerator;
        private string _currentIpAddress = "";
        private readonly string _torExePath = @"C:\Users\Administrator\Desktop\Tor Browser\Browser\TorBrowser\Tor\tor.exe";

        public Form1()
        {
            InitializeComponent();
            _torManager = new TorManager();
            _fingerprintGenerator = new FingerprintGenerator();
            UpdateUI();
        }

        private async void BtnConnect_Click(object? sender, EventArgs e)
        {
            if (_isConnected)
            {
                // Disconnect
                await DisconnectAsync();
            }
            else
            {
                // Connect
                await ConnectAsync();
            }
        }

        private async Task ConnectAsync()
        {
            try
            {
                btnConnect.Enabled = false;
                Log("Opening Tor Browser...");
                lblStatus.Text = "Status: Connecting...";

                // Start Tor process
                await StartTorProcessAsync();

                // Wait for Tor to initialize
                Log("Waiting for Tor initialization...");
                await Task.Delay(18000); // 18 seconds as in ScraperWorker

                // Get IP address through Tor
                Log("Connecting to Tor...");
                _currentIpAddress = await _torManager.GetCurrentIpAddressAsync();

                if (_currentIpAddress == "Unknown")
                {
                    Log("Error: Could not retrieve IP address. Tor may not be ready.");
                    lblStatus.Text = "Status: Error - Tor not ready";
                    await DisconnectAsync();
                    return;
                }

                Log($"New IP address: {_currentIpAddress}");
                UpdateIpAddress(_currentIpAddress);

                // Get GeoLocation for fingerprint
                GeoLocation? geo = await GetGeoLocationAsync();
                if (geo == null)
                {
                    Log("Warning: Could not retrieve geo data. Using default fingerprint.");
                    geo = new GeoLocation { Timezone = "UTC", CountryCode = "US" };
                }

                // Generate fingerprint
                FingerprintProfile profile = _fingerprintGenerator.GenerateWithGeo(geo);
                Log("New fingerprint generated:");
                Log($"  UserAgent: {profile.UserAgent}");
                Log($"  Language: {profile.Language}");
                Log($"  Timezone: {profile.Timezone}");
                Log($"  Platform: {profile.Platform}");
                Log($"  Screen: {profile.ScreenWidth}x{profile.ScreenHeight}");
                Log($"  DeviceMemory: {profile.DeviceMemory}");
                Log($"  HardwareConcurrency: {profile.HardwareConcurrency}");

                // Open browser with fingerprint
                Log("Opening Chrome Browser with new fingerprint...");
                _browserDriver = await StartBrowserWithFingerprintAsync(profile);

                if (_browserDriver == null)
                {
                    Log("Error: Browser could not be started.");
                    lblStatus.Text = "Status: Error - Browser could not be started";
                    await DisconnectAsync();
                    return;
                }

                _isConnected = true;
                UpdateUI();
                lblStatus.Text = "Status: Connected";
                Log("Browser successfully opened. Ready for use.");

                // Monitor browser process
                _ = Task.Run(MonitorBrowserProcessAsync);
            }
            catch (Exception ex)
            {
                Log($"Error connecting: {ex.Message}");
                Log($"Stack Trace: {ex.StackTrace}");
                lblStatus.Text = "Status: Error";
                await DisconnectAsync();
            }
            finally
            {
                btnConnect.Enabled = true;
            }
        }

        private async Task DisconnectAsync()
        {
            try
            {
                Log("Disconnecting...");

                // Close browser
                if (_browserDriver != null)
                {
                    try
                    {
                        _browserDriver.Quit();
                        _browserDriver.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Log($"Error closing browser: {ex.Message}");
                    }
                    _browserDriver = null;
                }

                // Close Tor process
                KillTorProcess();

                _isConnected = false;
                _currentIpAddress = "";
                UpdateIpAddress("Waiting for connection...");
                UpdateUI();
                lblStatus.Text = "Status: Disconnected";
                Log("Disconnected. Connect button ready for new connection.");
            }
            catch (Exception ex)
            {
                Log($"Error disconnecting: {ex.Message}");
            }
        }

        private async Task StartTorProcessAsync()
        {
            try
            {
                // Kill existing Tor process if running
                KillTorProcess();

                if (!File.Exists(_torExePath))
                {
                    Log($"Error: tor.exe not found at: {_torExePath}");
                    throw new FileNotFoundException($"tor.exe not found: {_torExePath}");
                }

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = _torExePath,
                    UseShellExecute = false,
                    CreateNoWindow = false
                };

                _torProcess = Process.Start(startInfo);
                if (_torProcess != null)
                {
                    Log($"Tor.exe started: {_torExePath}");
                    _torProcess.EnableRaisingEvents = true;
                    _torProcess.Exited += (sender, e) =>
                    {
                        Log("Tor.exe process terminated.");
                        _torProcess = null;
                    };
                }
            }
            catch (Exception ex)
            {
                Log($"Error starting Tor.exe: {ex.Message}");
                throw;
            }
        }

        private void KillTorProcess()
        {
            try
            {
                if (_torProcess != null && !_torProcess.HasExited)
                {
                    _torProcess.Kill();
                    _torProcess.WaitForExit(5000);
                    Log("Tor.exe terminated.");
                }
                else
                {
                    // Try to kill by process name as fallback
                    Process[] processes = Process.GetProcessesByName("tor");
                    foreach (Process proc in processes)
                    {
                        try
                        {
                            proc.Kill();
                            proc.WaitForExit(5000);
                            Log("Tor.exe terminated (by process name).");
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Error terminating Tor.exe: {ex.Message}");
            }
            finally
            {
                _torProcess = null;
            }
        }

        private async Task<IWebDriver?> StartBrowserWithFingerprintAsync(FingerprintProfile profile)
        {
            try
            {
                var options = new ChromeOptions();
                options.AddArgument("--disable-blink-features=AutomationControlled");
                options.AddExcludedArgument("enable-automation");
                options.AddArgument("--disable-popup-blocking");

                // Tor Proxy
                options.AddArgument("--proxy-server=socks5://127.0.0.1:9050");

                // Set Chrome binary location
                SetChromeBinaryLocation(options);

                var driver = new ChromeDriver(options);
                var devTools = driver as IDevTools;
                if (devTools == null) throw new InvalidOperationException("DevTools not available.");

                var session = devTools.GetDevToolsSession();
                var domains = session.GetVersionSpecificDomains<OpenQA.Selenium.DevTools.V143.DevToolsSessionDomains>();

                // Set User-Agent and Language
                await domains.Emulation.SetUserAgentOverride(new()
                {
                    UserAgent = profile.UserAgent,
                    AcceptLanguage = profile.Language.Split(',')[0],
                    Platform = profile.Platform
                });

                // Set Timezone
                await domains.Emulation.SetTimezoneOverride(new() { TimezoneId = profile.Timezone });

                // Inject fingerprint script
                string fingerprintScript = BuildFingerprintInjectionScript(profile);
                driver.ExecuteCdpCommand("Page.addScriptToEvaluateOnNewDocument", new Dictionary<string, object>
                {
                    ["source"] = fingerprintScript
                });

                // Prevent new tabs - force new windows
                string preventNewTabScript = @"
                (() => {
                    // 1. Modify all <a> tags - remove target='_blank'
                    document.addEventListener('DOMContentLoaded', () => {
                        const links = document.querySelectorAll('a[target=""_blank""]');
                        links.forEach(link => {
                            link.removeAttribute('target');
                        });
                    });

                    // 2. Override window.open - block new tabs, only allow new windows
                    const originalOpen = window.open;
                    window.open = function(url, name, specs) {
                        if (name === '_blank' || name === '' || !name) {
                            // If no name or _blank, open in same tab
                            if (url) {
                                window.location.href = url;
                            }
                            return window;
                        }
                        // If a specific name, allow new window
                        return originalOpen.call(this, url, name, specs);
                    };

                    // 3. Prevent Ctrl+T, Ctrl+N for new tabs
                    document.addEventListener('keydown', function(e) {
                        if ((e.ctrlKey || e.metaKey) && e.key === 't') {
                            e.preventDefault();
                            e.stopPropagation();
                            return false;
                        }
                        if ((e.ctrlKey || e.metaKey) && e.key === 'n') {
                            e.preventDefault();
                            e.stopPropagation();
                            return false;
                        }
                    }, true);
                })();
                ";
                driver.ExecuteCdpCommand("Page.addScriptToEvaluateOnNewDocument", new Dictionary<string, object>
                {
                    ["source"] = preventNewTabScript
                });

                driver.Manage().Window.Size = new Size(profile.ScreenWidth, profile.ScreenHeight);
                driver.Navigate().GoToUrl("about:blank");

                return driver;
            }
            catch (Exception ex)
            {
                Log($"Browser start error: {ex.Message}");
                return null;
            }
        }

        private string BuildFingerprintInjectionScript(FingerprintProfile fp)
        {
            return $@"
(() => {{
// Disable WebRTC to prevent IP leaks
    const OriginalRTCPeerConnection = window.RTCPeerConnection || window.webkitRTCPeerConnection;
    if (OriginalRTCPeerConnection) {{
        class SpoofedRTCPeerConnection extends OriginalRTCPeerConnection {{
            constructor(config) {{
                super(config);
                this.addIceCandidate = function() {{
                    return Promise.resolve();
                }};
                this.createOffer = function() {{
                    return Promise.resolve({{ type: 'offer', sdp: '' }});
                }};
                this.createAnswer = function() {{
                    return Promise.resolve({{ type: 'answer', sdp: '' }});
                }};
            }}
        }}
        window.RTCPeerConnection = SpoofedRTCPeerConnection;
        if (window.webkitRTCPeerConnection) {{
            window.webkitRTCPeerConnection = SpoofedRTCPeerConnection;
        }}
    }}
    if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {{
        navigator.mediaDevices.getUserMedia = function() {{
            return Promise.reject(new Error('getUserMedia is disabled for privacy reasons'));
        }};
    }}
    // Navigator properties
    Object.defineProperty(navigator, 'userAgent', {{ get: () => '{fp.UserAgent}' }});
    Object.defineProperty(navigator, 'language', {{ get: () => '{fp.Language.Split(',')[0]}' }});
    Object.defineProperty(navigator, 'languages', {{ get: () => ['{string.Join("','", fp.Language.Split(','))}'] }});
    Object.defineProperty(navigator, 'platform', {{ get: () => '{fp.Platform}' }});
    Object.defineProperty(navigator, 'deviceMemory', {{ get: () => {fp.DeviceMemory} }});
    Object.defineProperty(navigator, 'hardwareConcurrency', {{ get: () => {fp.HardwareConcurrency} }});
    Object.defineProperty(navigator, 'maxTouchPoints', {{ get: () => {fp.MaxTouchPoints} }});
    Object.defineProperty(navigator, 'doNotTrack', {{ get: () => '{fp.DoNotTrack}' }});

    // Screen properties
    Object.defineProperty(screen, 'width', {{ get: () => {fp.ScreenWidth} }});
    Object.defineProperty(screen, 'height', {{ get: () => {fp.ScreenHeight} }});
    Object.defineProperty(screen, 'availWidth', {{ get: () => {fp.ScreenAvailableWidth} }});
    Object.defineProperty(screen, 'availHeight', {{ get: () => {fp.ScreenAvailableHeight} }});
    Object.defineProperty(screen, 'colorDepth', {{ get: () => {fp.ColorDepth} }});
    Object.defineProperty(navigator, 'locale', {{ get: () => '{fp.Locale}',configurable: true }});

    // Storage Features
    {(fp.LocalStorage ? "" : "Object.defineProperty(window, 'localStorage', { get: () => undefined, configurable: true });")}
    {(fp.SessionStorage ? "" : "Object.defineProperty(window, 'sessionStorage', { get: () => undefined, configurable: true });")}
    {(fp.IndexedDB ? "" : "Object.defineProperty(window, 'indexedDB', { get: () => undefined, configurable: true });")}

    // WebGL Spoofing
    try {{
        const original = WebGLRenderingContext.prototype.getParameter;
        WebGLRenderingContext.prototype.getParameter = function(param) {{
            if (param === 37445) return '{fp.WebGLVendor}';
            if (param === 37446) return '{fp.WebGLRenderer}';
            return original.apply(this, arguments);
        }};
    }} catch (e) {{
        console.error('WebGL spoofing failed:', e);
    }}

    // Plugins
    try {{
        Object.defineProperty(navigator, 'plugins', {{
            get: () => ['{string.Join("','", fp.Plugins)}']
        }});
    }} catch(e) {{
        console.error('Navigator.plugins spoofing failed:', e);
    }}

    window.__audioFingerprint = '{fp.AudioFingerprint}';
    window.__canvasFingerprint = '{fp.CanvasFingerprint}';
}})();
";
        }

        private static void SetChromeBinaryLocation(ChromeOptions options)
        {
            var chromePaths = new[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Google", "Chrome", "Application", "chrome.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Google", "Chrome", "Application", "chrome.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Google", "Chrome", "Application", "chrome.exe")
            };

            foreach (var chromePath in chromePaths)
            {
                if (File.Exists(chromePath))
                {
                    options.BinaryLocation = chromePath;
                    break;
                }
            }
        }

        private async Task<GeoLocation?> GetGeoLocationAsync()
        {
            try
            {
                // Use Tor proxy to get geo location
                var proxy = new MihaZupan.HttpToSocks5Proxy("127.0.0.1", 9050);
                var handler = new HttpClientHandler
                {
                    Proxy = proxy,
                    UseProxy = true
                };
                using var client = new HttpClient(handler);
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
                client.Timeout = TimeSpan.FromSeconds(30);

                var response = await client.GetAsync("http://ip-api.com/json");
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(content).RootElement;

                return new GeoLocation
                {
                    Country = doc.GetProperty("country").GetString() ?? "",
                    RegionName = doc.GetProperty("regionName").GetString() ?? "",
                    City = doc.GetProperty("city").GetString() ?? "",
                    Timezone = doc.GetProperty("timezone").GetString() ?? "",
                    CountryCode = doc.GetProperty("countryCode").GetString() ?? "",
                    Query = doc.GetProperty("query").GetString() ?? ""
                };
            }
            catch (Exception ex)
            {
                Log($"GeoLocation error: {ex.Message}");
                return null;
            }
        }

        private async Task MonitorBrowserProcessAsync()
        {
            try
            {
                while (_isConnected && _browserDriver != null)
                {
                    await Task.Delay(2000); // Check every 2 seconds

                    try
                    {
                        // Try to get window handle - if this fails, browser is closed
                        var handles = _browserDriver.WindowHandles;
                        if (handles.Count == 0)
                        {
                            // All windows closed
                            Log("Browser was closed (all windows closed).");
                            if (InvokeRequired)
                            {
                                Invoke(new Action(async () => await DisconnectAsync()));
                            }
                            else
                            {
                                await DisconnectAsync();
                            }
                            break;
                        }
                    }
                    catch (WebDriverException)
                    {
                        // Browser is closed or crashed
                        Log("Browser was closed or crashed.");
                        if (InvokeRequired)
                        {
                            Invoke(new Action(async () => await DisconnectAsync()));
                        }
                        else
                        {
                            await DisconnectAsync();
                        }
                        break;
                    }
                    catch (Exception ex)
                    {
                        // Other exceptions - log and continue monitoring
                        Log($"Warning during browser monitoring: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Error during browser monitoring: {ex.Message}");
            }
        }

        private void UpdateUI()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateUI));
                return;
            }

            btnConnect.Text = _isConnected ? "Disconnect" : "Connect";
            btnConnect.Enabled = true;
        }

        private void UpdateIpAddress(string ip)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateIpAddress(ip)));
                return;
            }

            txtIpAddress.Text = ip;
        }

        private void Log(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => Log(message)));
                return;
            }

            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            richTextBoxLog.AppendText($"[{timestamp}] {message}\r\n");
            richTextBoxLog.ScrollToCaret();
        }

        private async void Form1_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (_isConnected)
            {
                await DisconnectAsync();
            }
            _torManager?.Dispose();
        }
    }
}
