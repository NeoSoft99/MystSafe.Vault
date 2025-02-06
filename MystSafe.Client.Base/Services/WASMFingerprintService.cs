// MystSafe is a secret vault with anonymous access and zero activity tracking protected by cryptocurrency-grade tech.
// 
//     Copyright (C) 2024-2025 MystSafe, NeoSoft99
// 
//     MystSafe: The Only Privacy-Preserving Password Manager.
//     https://mystsafe.com
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU Affero General Public License as
//     published by the Free Software Foundation, either version 3 of the
//     License, or (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
//     See the GNU Affero General Public License for more details.
// 
//     You should have received a copy of the GNU Affero General Public License
//     along with this program.  If not, see <https://www.gnu.org/licenses/>.

using Microsoft.JSInterop;
using System.Text.RegularExpressions;

namespace MystSafe.Client.Base;

public class WASMFingerprintService: IRuntimeFingerprintService
{

    private readonly IJSRuntime _jsRuntime;

    public WASMFingerprintService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<string> GetRuntimeFingerprintAsync()
    {
        try
        {

            var browserInfo = await _jsRuntime.InvokeAsync<RuntimeBrowserInfo>("getBrowserInfo");
            var usInfo = ParseUserAgent(browserInfo.UserAgent);
            var IsMobile = await _jsRuntime.InvokeAsync<bool>("window.isMobile");

            var concat =
                browserInfo.DeviceMemory +
                browserInfo.HardwareConcurrency +
                browserInfo.Language +
                browserInfo.Platform +
                usInfo.BrowserName +
                usInfo.OS +
                IsMobile;

            //var hash = Hashing.SHA256Base64(concat);

            return concat;
        }
        catch (Exception e)
        {
            return string.Empty;
        }

    }


  


    private RuntimeUserAgentInfo ParseUserAgent(string userAgent)
    {
        var info = new RuntimeUserAgentInfo
        {
            BrowserName = "Unknown",
            OS = "Unknown"
        };

        // Browser name and version extraction
        if (userAgent.Contains("Edg"))
        {
            info.BrowserName = "Edge";
        }
        else if (userAgent.Contains("Firefox"))
        {
            info.BrowserName = "Firefox";
        }
        else if (userAgent.Contains("OPR") || userAgent.Contains("Opera"))
        {
            info.BrowserName = "Opera";
        }
        else if (userAgent.Contains("Chrome"))
        {
            info.BrowserName = "Chrome";
        }
        else if (userAgent.Contains("Safari"))
        {
            info.BrowserName = "Safari";
        }
        else if (userAgent.Contains("MSIE"))
        {
            info.BrowserName = "Internet Explorer";
        }
        else if (userAgent.Contains("Trident"))
        {
            info.BrowserName = "Internet Explorer";
        }

        // Operating system extraction
        if (Regex.IsMatch(userAgent, "Windows", RegexOptions.IgnoreCase))
        {
            info.OS = "Windows";
        }
        else if (Regex.IsMatch(userAgent, "Macintosh", RegexOptions.IgnoreCase))
        {
            info.OS = "MacOS";
        }
        else if (Regex.IsMatch(userAgent, "Linux", RegexOptions.IgnoreCase))
        {
            info.OS = "Linux";
        }
        else if (Regex.IsMatch(userAgent, "Android", RegexOptions.IgnoreCase))
        {
            info.OS = "Android";
        }
        else if (Regex.IsMatch(userAgent, "iOS", RegexOptions.IgnoreCase) || Regex.IsMatch(userAgent, "iPhone|iPad|iPod", RegexOptions.IgnoreCase))
        {
            info.OS = "iOS";
        }

        return info;
    }

}

internal class RuntimeUserAgentInfo
{
    public string? BrowserName { get; set; }
    public string? OS { get; set; }
}

internal class RuntimeBrowserInfo
{
    public double? DeviceMemory { get; set; }
    public string? UserAgent { get; set; }
    public int? HardwareConcurrency { get; set; }
    public string? Platform { get; set; }
    public string? Language { get; set; }

}






