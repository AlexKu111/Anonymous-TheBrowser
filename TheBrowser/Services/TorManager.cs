using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using MihaZupan;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace TheBrowser.Services;

public class TorManager
{
    private const string TorProxyHost = "127.0.0.1";
    private const int TorProxyPort = 9050;
    private const string TorSocksProxy = "socks5://127.0.0.1:9050";
    private readonly HttpClient _httpClient;

    public TorManager()
    {
        // Use HttpToSocks5Proxy for proper SOCKS5 support
        var proxy = new HttpToSocks5Proxy(TorProxyHost, TorProxyPort);
        var handler = new HttpClientHandler
        {
            Proxy = proxy,
            UseProxy = true
        };
        _httpClient = new HttpClient(handler);
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    public async Task<string> GetCurrentIpAddressAsync()
    {
        try
        {
            var response = await _httpClient.GetStringAsync("https://api.ipify.org");
            return response.Trim();
        }
        catch (System.Net.Sockets.SocketException)
        {
            // Tor connection error - silently return Unknown
            return "Unknown";
        }
        catch (Exception ex)
        {
            // Only log non-socket exceptions
            Console.WriteLine($"Error getting IP address: {ex.Message}");
            return "Unknown";
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}

