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

using System.Text;
using Microsoft.Extensions.Logging;
using MystSafe.Shared.Common;
using MystSafe.Shared.CryptoLicense;
using Newtonsoft.Json;

namespace MystSafe.Client.Engine;

public class ApiClientService
{
    private readonly HttpClient _apiClient;
    private readonly string _backEndURL;
    private readonly ILogger<ApiClientService> _logger;
    
    public string BackEndURL => _backEndURL;
    public HttpClient APIClient => _apiClient;

    public ApiClientService(ILogger<ApiClientService> logger, string backEndURL)
    {
        _logger = logger;
        _backEndURL = IncrementLocalhostPort(backEndURL); // for local debug only increment the port number 
        _apiClient = new HttpClient();
    }
    
    private static string IncrementLocalhostPort(string url)
    {
        try
        {
            // Parse the URL
            var uri = new Uri(url);

            // Check if the host is "localhost"
            if (uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase))
            {
                int newPort; 
                if (uri.Port == 80 || uri.Port == 6002)   
                {
                    newPort = 80; // if the client is running on production or in debug with relay running in local k3
                }
                else
                {
                    newPort = uri.Port + 1; // if the relay is running in debugger
                }



                // Create a new UriBuilder to modify the port
                var uriBuilder = new UriBuilder(uri)
                {
                    Port = newPort
                };
                // Return the modified URL as a string
                return uriBuilder.ToString();
            }
        }
        catch (UriFormatException)
        {
            Console.WriteLine("The provided URL is not in a valid format.");
        }

        // Return the original URL if the host is not "localhost" or if an exception occurs
        return url;
    }

    #region Secrets

    public async Task<SecretBlockBroadcastResult> SecretBroadcastAsync(SecretBlock block)
    {
        try
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(block);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var server_url = _backEndURL;
            var request_url = $"{server_url}api/SecretBlock/Broadcast";

            var response = await _apiClient.PostAsync(request_url, data);
            Console.WriteLine(request_url);

            if (response.IsSuccessStatusCode)
            {
                var resultJson = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SecretBlockBroadcastResult>(resultJson);
                if (result is null)
                    throw new Exception("api call response: null");
                return result;
            }
            else
            {
                throw new Exception("api call response: " + response);
            }
        }
        catch (Exception ex)
        {
            throw new Exception("api call failed: " + ex.Message);
        }
    }

    public async Task<SecretBlockScanResult> SecretScanAsync(ScanSecretBlockParams scan_params)
    {
        try
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(scan_params);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var server_url = _backEndURL;
            var request_url = $"{server_url}api/SecretBlock/Scan";

            var response = await _apiClient.PostAsync(request_url, data);
            Console.WriteLine(request_url);

            if (response.IsSuccessStatusCode)
            {
                var resultJson = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SecretBlockScanResult>(resultJson);
                if (result is null)
                    throw new Exception("api call response: null");
                return result;
            }
            else
            {
                throw new Exception("api call response: " + response);
            }
        }
        catch (Exception ex)
        {
            throw new Exception("api call failed: " + ex.Message);
        }
    }

    public async Task<SecretBlockResult> SecretGetByHashAsync(SecretBlockParams get_params)
    {
        try
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(get_params);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var server_url = _backEndURL;
            var request_url = $"{server_url}api/SecretBlock/GetByHash";

            var response = await _apiClient.PostAsync(request_url, data);
            Console.WriteLine(request_url);

            if (response.IsSuccessStatusCode)
            {
                var resultJson = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SecretBlockResult>(resultJson);
                if (result is null)
                    throw new Exception("api call response: null");
                return result;
            }
            else
            {
                throw new Exception("api call response: " + response);
            }
        }
        catch (Exception ex)
        {
            throw new Exception("api call failed: " + ex.Message);
        }
    }

    #endregion

    #region Contacts

    public async Task<BroadcastContactResult> ContactBroadcastAsync(ContactBlock block)
    {
        try
        {
            var json = JsonConvert.SerializeObject(block);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var requestUrl = $"{_backEndURL}api/ContactBlock/Broadcast";

            var response = await _apiClient.PostAsync(requestUrl, data);
            Console.WriteLine(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                var resultJson = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<BroadcastContactResult>(resultJson);
                if (result == null)
                    throw new Exception("API call response: null");
                return result;
            }
            else
            {
                throw new Exception("API call response: " + response);
            }
        }
        catch (Exception ex)
        {
            throw new Exception("API call failed: " + ex.Message);
        }
    }

    public async Task<ScanContactResult> ContactScanAsync(ScanContactParams scanParams)
    {
        try
        {
            var json = JsonConvert.SerializeObject(scanParams);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var requestUrl = $"{_backEndURL}api/ContactBlock/Scan";

            var response = await _apiClient.PostAsync(requestUrl, data);
            Console.WriteLine(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                var resultJson = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ScanContactResult>(resultJson);
                if (result == null)
                    throw new Exception("API call response: null");
                return result;
            }
            else
            {
                throw new Exception("API call response: " + response);
            }
        }
        catch (Exception ex)
        {
            throw new Exception("API call failed: " + ex.Message);
        }
    }

    #endregion

    #region InitBlock

    // Assuming ApiClientService is already defined and you're adding to it

    public async Task<AddInitBlockStatus> InitBlockBroadcastAsync(InitBlock block)
    {
        var json = JsonConvert.SerializeObject(block);
        var data = new StringContent(json, Encoding.UTF8, "application/json");
        var requestUrl = $"{_backEndURL}api/InitBlock/Broadcast";
        var response = await _apiClient.PostAsync(requestUrl, data);

        if (response.IsSuccessStatusCode)
        {
            var resultJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<AddInitBlockStatus>(resultJson);
        }
        else
        {
            throw new Exception("API call response: " + response);
        }
    }

    public async Task<ScanChatResult> InitBlockScanAsync(ChatScanParams scanParams)
    {
        var json = JsonConvert.SerializeObject(scanParams);
        var data = new StringContent(json, Encoding.UTF8, "application/json");
        var requestUrl = $"{_backEndURL}api/InitBlock/Scan";
        var response = await _apiClient.PostAsync(requestUrl, data);

        if (response.IsSuccessStatusCode)
        {
            var resultJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ScanChatResult>(resultJson);
        }
        else
        {
            throw new Exception("API call response: " + response);
        }
    }

    #endregion

    #region Messages

    // Assuming ApiClientService is already defined and you're adding to it

    public async Task<AddMsgBlockStatus> MsgBlockBroadcastAsync(MsgBlock block)
    {
        var json = JsonConvert.SerializeObject(block);
        var data = new StringContent(json, Encoding.UTF8, "application/json");
        var requestUrl = $"{_backEndURL}api/MsgBlock/Broadcast";
        var response = await _apiClient.PostAsync(requestUrl, data);

        if (response.IsSuccessStatusCode)
        {
            var resultJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<AddMsgBlockStatus>(resultJson);
        }
        else
        {
            throw new Exception("API call response: " + response);
        }
    }

    public async Task<ScanMsgResult> MsgBlockScanAsync(ScanMsgParams scanParams)
    {
        var json = JsonConvert.SerializeObject(scanParams);
        var data = new StringContent(json, Encoding.UTF8, "application/json");
        var requestUrl = $"{_backEndURL}api/MsgBlock/Scan";
        var response = await _apiClient.PostAsync(requestUrl, data);

        if (response.IsSuccessStatusCode)
        {
            var resultJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ScanMsgResult>(resultJson);
        }
        else
        {
            throw new Exception("API call response: " + response);
        }
    }

    #endregion
    
    
}


