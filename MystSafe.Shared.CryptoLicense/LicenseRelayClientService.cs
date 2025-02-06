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

using Microsoft.Extensions.Logging;
using System.Text;
using Newtonsoft.Json;
    
namespace MystSafe.Shared.CryptoLicense;

public class LicenseRelayClientService: ILicenseRelayClientService
{
    private readonly HttpClient _apiClient;
    private readonly string _backEndURL;
    private readonly ILogger<LicenseRelayClientService> _logger;

    public LicenseRelayClientService(ILogger<LicenseRelayClientService> logger, string backEndURL, HttpClient apiClient)
    {
        _logger = logger;
        _backEndURL = backEndURL;
        _apiClient = apiClient;
    }

    public async Task<LicenseTransactionValidationResponse> ValidateLicenseTransaction(string txPubKey)
    {
        try
        {
            var request = new ValidateLicenseTxRequest();
            request.TxPubKey = txPubKey;
            var json = JsonConvert.SerializeObject(request);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var requestUrl = $"{_backEndURL}api/CryptoLicense/ValidateLicenseTransaction";
            var response = await _apiClient.PostAsync(requestUrl, data);

            if (response.IsSuccessStatusCode)
            {
                var resultJson = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<LicenseTransactionValidationResponse>(resultJson);
                if (result is null)
                    throw new Exception("ValidateLicenseTransaction API call response: null");
                
                return result;
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to validate license transaction. Server responded with: {0}", errorResponse);
                throw new Exception("Failed to validate license transaction. Server response: " + errorResponse);
            }
        }
        catch (Exception ex)
        {
            throw new Exception(" ValidateLicenseTransaction api call failed: " + ex.Message, ex);
        }
    }
    
    public async Task<GetTransactionsResponse> GetTransactionsSince(GetTransactionsSinceRequest request)
    {
        try
        {
            var json = JsonConvert.SerializeObject(request);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var requestUrl = $"{_backEndURL}api/CryptoLicense/GetTransactionsSince";
            var response = await _apiClient.PostAsync(requestUrl, data);

            if (response.IsSuccessStatusCode)
            {
                var resultJson = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<GetTransactionsResponse>(resultJson);
                if (result is null)
                    throw new Exception("GetTransactionsSince API call response: null");

                return result;
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to get transactions. Server responded with: {0}", errorResponse);
                throw new Exception("Failed to get transactions. Server response: " + errorResponse);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("GetTransactionsSince API call failed: {0}", ex.Message);
            throw new Exception("GetTransactionsSince API call failed: " + ex.Message, ex);
        }
    }

    private static string IncrementLocalhostPort(string url)
    {
        try
        {
            // Parse the URL
            var uri = new Uri(url);
            var newPort = uri.Port + 1;

            // Create a new UriBuilder to modify the port
            var uriBuilder = new UriBuilder(uri)
            {
                Port = newPort
            };
            // Return the modified URL as a string
            return uriBuilder.ToString();
        }
        catch (UriFormatException)
        {
            Console.WriteLine("The provided URL is not in a valid format.");
        }

        // Return the original URL if the host is not "localhost" or if an exception occurs
        return url;
    }

    public async Task<GetRecentTransactionResponse> GetRecentTransaction()
    {
        try
        {
            var requestUrl = $"{_backEndURL}api/CryptoLicense/GetRecentTransaction";

            var response = await _apiClient.PostAsync(requestUrl, null);

            if (response.IsSuccessStatusCode)
            {
                var resultJson = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<GetRecentTransactionResponse>(resultJson);
                if (result is null)
                    throw new Exception("GetRecentTransaction API call response: null");
                
                return result;
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to get recent transaction. Server responded with: {0}", errorResponse);
                throw new Exception("Failed to get recent transaction. Server response: " + errorResponse);
            }
        }
        catch (Exception ex)
        {
            throw new Exception(" GetRecentTransaction api call failed: " + ex.Message, ex);
        }
    }
    
    public async Task<FetchDecoyOutputsResponse> FetchDecoyOutputs(FetchDecoyOutputsRequest request)
    {
        try
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var requestUrl = $"{_backEndURL}api/CryptoLicense/FetchDecoyOutputs";

            var response = await _apiClient.PostAsync(requestUrl, data);

            if (response.IsSuccessStatusCode)
            {
                var resultJson = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<FetchDecoyOutputsResponse>(resultJson);
                if (result is null)
                    throw new Exception("FetchDecoyOutputs API call response: null");
                return result;
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to fetch decoy outputs. Server responded with: {0}", errorResponse);
                throw new Exception("Failed to fetch decoy outputs. Server response: " + errorResponse);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("FetchDecoyOutputs API call failed: {0}", ex.Message);
            throw new Exception("FetchDecoyOutputs API call failed: " + ex.Message, ex);
        }
    }
    
    public async Task<BaseLicenseResponse> AddTransaction(TxDTO transactionDto)
    {
        try
        {
            AddTransactionRequest request = new AddTransactionRequest();
            request.Transaction = transactionDto;
            var json = JsonConvert.SerializeObject(request);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var requestUrl = $"{_backEndURL}api/CryptoLicense/AddTransaction";

            var response = await _apiClient.PostAsync(requestUrl, data);

            if (response.IsSuccessStatusCode)
            {
                var resultJson = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<BaseLicenseResponse>(resultJson);
                if (result is null)
                    throw new Exception("AddTransaction API call response: null");

                _logger.LogInformation("Transaction added successfully.");
                return result;
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to add transaction. Server responded with: {0}", errorResponse);
                throw new Exception("Failed to add transaction. Server response: " + errorResponse);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("AddTransaction API call failed: {0}", ex.Message);
            throw new Exception("AddTransaction API call failed: " + ex.Message, ex);
        }
    }

}