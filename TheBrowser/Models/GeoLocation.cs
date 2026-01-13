namespace TheBrowser.Models;

public class GeoLocation
{
    public string Country { get; set; } = "";
    public string RegionName { get; set; } = "";
    public string City { get; set; } = "";
    public string Timezone { get; set; } = "";
    public string CountryCode { get; set; } = "";
    public string Query { get; set; } = ""; // IP


    public string LangCode => CountryCode switch
    {
        "DE" => "de-DE,de;q=0.9",
        "AT" => "de-AT,de;q=0.9",
        "CH" => "de-CH,de;q=0.9",

        "US" => "en-US,en;q=0.9",
        "GB" => "en-GB,en;q=0.9",
        "AU" => "en-AU,en;q=0.9",
        "CA" => "en-CA,en;q=0.9",

        "FR" => "fr-FR,fr;q=0.9",
        "BE" => "fr-BE,fr;q=0.9",
        "CA-FR" => "fr-CA,fr;q=0.9",

        "IT" => "it-IT,it;q=0.9",
        "ES" => "es-ES,es;q=0.9",
        "MX" => "es-MX,es;q=0.9",

        "RU" => "ru-RU,ru;q=0.9",
        "UA" => "uk-UA,uk;q=0.9",
        "PL" => "pl-PL,pl;q=0.9",
        "NL" => "nl-NL,nl;q=0.9",
        "SE" => "sv-SE,sv;q=0.9",
        "FI" => "fi-FI,fi;q=0.9",
        "NO" => "no-NO,no;q=0.9",
        "DK" => "da-DK,da;q=0.9",

        "JP" => "ja-JP,ja;q=0.9",
        "CN" => "zh-CN,zh;q=0.9",
        "HK" => "zh-HK,zh;q=0.9",
        "TW" => "zh-TW,zh;q=0.9",
        "KR" => "ko-KR,ko;q=0.9",

        "TR" => "tr-TR,tr;q=0.9",
        "CZ" => "cs-CZ,cs;q=0.9",
        "SK" => "sk-SK,sk;q=0.9",
        "HU" => "hu-HU,hu;q=0.9",
        "RO" => "ro-RO,ro;q=0.9",
        "GR" => "el-GR,el;q=0.9",

        "IL" => "he-IL,he;q=0.9",
        "SA" => "ar-SA,ar;q=0.9",
        "IR" => "fa-IR,fa;q=0.9",
        "IN" => "hi-IN,hi;q=0.9",

        _ => "en-US,en;q=0.9"
    };

    public string Locale => CountryCode switch
    {
        "DE" => "de-DE",
        "AT" => "de-AT",
        "CH" => "de-CH",
        "US" => "en-US",
        "GB" => "en-GB",
        "AU" => "en-AU",
        "CA" => "en-CA",
        "FR" => "fr-FR",
        "BE" => "fr-BE",
        "CA-FR" => "fr-CA",
        "IT" => "it-IT",
        "ES" => "es-ES",
        "MX" => "es-MX",
        "RU" => "ru-RU",
        "UA" => "uk-UA",
        "PL" => "pl-PL",
        "NL" => "nl-NL",
        "SE" => "sv-SE",
        "FI" => "fi-FI",
        "NO" => "no-NO",
        "DK" => "da-DK",
        "JP" => "ja-JP",
        "CN" => "zh-CN",
        "HK" => "zh-HK",
        "TW" => "zh-TW",
        "KR" => "ko-KR",
        "TR" => "tr-TR",
        "CZ" => "cs-CZ",
        "SK" => "sk-SK",
        "HU" => "hu-HU",
        "RO" => "ro-RO",
        "GR" => "el-GR",
        "IL" => "he-IL",
        "SA" => "ar-SA",
        "IR" => "fa-IR",
        "IN" => "hi-IN",
        _ => "en-US"
    };
}

