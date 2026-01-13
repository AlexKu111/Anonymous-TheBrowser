# TheBrowser

A Windows desktop application that launches Chrome browser instances through the Tor network with advanced fingerprint spoofing capabilities for enhanced privacy and anonymity.

## Overview

TheBrowser is a C# Windows Forms application that automates the process of:
- Starting Tor Browser and routing traffic through the Tor network
- Generating unique browser fingerprints to prevent tracking
- Launching Chrome instances with spoofed browser characteristics
- Automatically configuring browser properties to match geographic locations
  

## How It Works
1. **Tor Connection**: The application starts the Tor process and waits for initialization
2. **IP Detection**: Retrieves the current Tor exit node IP address
3. **Geolocation**: Determines the geographic location of the exit node
4. **Fingerprint Generation**: Creates a unique browser fingerprint matching the exit node's location
5. **Browser Launch**: Opens Chrome with the generated fingerprint and routes traffic through Tor
6. **Monitoring**: Continuously monitors the browser process and Tor connection


## Features

### 🔒 Privacy & Anonymity
- **Tor Integration**: All browser traffic is routed through the Tor network for anonymous browsing
- **IP Address Rotation**: Each connection uses a new Tor exit node IP address
- **Automatic IP Detection**: Displays the current Tor exit node IP address

### 🎭 Browser Fingerprinting Protection
- **Dynamic Fingerprint Generation**: Creates unique browser fingerprints from JSON profile files
- **Geographic Matching**: Automatically matches browser fingerprint to the Tor exit node's geographic location
- **Comprehensive Spoofing**: Modifies multiple browser properties including:
  - User-Agent strings
  - Language and locale settings
  - Screen resolution and color depth
  - Platform information
  - Device memory and hardware concurrency
  - WebGL vendor and renderer
  - Canvas and audio fingerprints
  - Navigator plugins
  - Timezone settings
  - Storage capabilities (LocalStorage, SessionStorage, IndexedDB)

### 🌐 Browser Automation
- **Chrome Integration**: Launches Chrome browser instances with Selenium WebDriver
- **Tab Prevention**: Automatically prevents new tabs from opening (forces new windows)
- **Process Monitoring**: Monitors browser process and automatically disconnects when closed
- **WebRTC Leak Prevention**: Disables WebRTC to prevent IP address leaks

## Requirements

### ⚠️ Important Prerequisites

**Tor Browser MUST be preinstalled** on your system before using this application.

The application expects Tor Browser to be installed at:
```
C:\Users\[USERNAME]\Desktop\Tor Browser\Browser\TorBrowser\Tor\tor.exe
```

You can modify the path in `Form1.cs` if your Tor Browser is installed in a different location.

### Additional Requirements
- **.NET 8.0** or later
- **Google Chrome** browser installed
- **Fingerprint JSON Files**: The application requires fingerprint profile files in your Documents folder:
  - `set1_teil1.json` through `set1_teil5.json`
  - `set2_teil1.json` through `set2_teil5.json`


## Usage

1. Ensure Tor Browser is installed (see Requirements above)
2. Place fingerprint JSON files in your Documents folder
3. Run the application
4. Click "Connect" to start a new Tor session
5. The application will:
   - Start Tor Browser
   - Display your new IP address
   - Generate and apply a unique fingerprint
   - Launch Chrome browser through Tor
6. Click "Disconnect" to close the browser and Tor connection

## Technical Details

### Architecture
- **Framework**: .NET 8.0 Windows Forms
- **Browser Automation**: Selenium WebDriver with ChromeDriver
- **Tor Integration**: Direct process management and SOCKS5 proxy configuration
- **Fingerprinting**: Custom fingerprint generation from JSON profile files

### Key Components
- `Form1.cs`: Main application window and connection logic
- `TorManager.cs`: Handles Tor proxy connections and IP address detection
- `FingerprintGenerator.cs`: Generates unique browser fingerprints from JSON files
- `FingerprintProfile.cs`: Data model for browser fingerprint properties
- `GeoLocation.cs`: Geographic location data and language/locale mappings

## Configuration

### Tor Browser Path
Update the `_torExePath` variable in `Form1.cs` if your Tor Browser is installed elsewhere:
```csharp
private readonly string _torExePath = @"C:\Path\To\Tor\tor.exe";
```

### Fingerprint Files
The application looks for fingerprint JSON files in:
```
%USERPROFILE%\Documents\
```

Required files:
- `set1_teil1.json` to `set1_teil5.json`
- `set2_teil2.json` to `set2_teil5.json`

## Security Notes

- This tool is designed for privacy and anonymity testing
- Always use responsibly and in accordance with applicable laws
- The fingerprint spoofing helps prevent tracking but does not guarantee complete anonymity
- Tor Browser must be kept up-to-date for security

