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

using MystSafe.Shared.Common;
using Fido2.BlazorWebAssembly;
using Fido2NetLib;

namespace MystSafe.Client.Base;

public class WebAuthnUser
{
    private readonly WebAuthn _webAuthn;
    private readonly WebAuthnServer _serverService;

    public WebAuthnUser(WebAuthn webAuthn, WebAuthnServer serverService)
    {
        _webAuthn = webAuthn;
        _serverService = serverService;
    }

    // returns results code and key seed
    public async Task<AuthnRegisterResult> RegisterAsync()
    {
        var result = new AuthnRegisterResult();

        // Now the magic happens so stuff can go wrong
        CredentialCreateOptions? options;
        string? secret_seed;
        try
        {
            // Make sure the WebAuthn API is initialized (although that should happen almost immediately after startup)
            await _webAuthn.Init();

            // Get options from server
            (options, secret_seed) = _serverService.GetCredentialOptions();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            result.ResultCode = ResultStatusCodes.EXCEPTION;
            result.ResultMessage = e.Message;
            return result;
        }

        if (options == null)
        {
            result.ResultCode = ResultStatusCodes.EXCEPTION;
            result.ResultMessage = "No options received";
            return result;
            
        }

        try
        {
            Console.WriteLine("CreateCredsAsync() options:");
            Console.WriteLine("CreateCredsAsync() options : " + options is not null ? options.ToJson() : "null");

            // Present options to user and get response
            var attestationResponse = await _webAuthn.CreateCredsAsync(options);

            Console.WriteLine("attestationResponse:" + attestationResponse.ToString());

            // Send response to server
            var (resultcode, credential_result) = await _serverService.CreateCredentialAsync(options, attestationResponse);
            result.ResultCode = resultcode;
            if (result.ResultCode != ResultStatusCodes.SUCCESS)
            {
                result.ResultMessage = "Error in CreateCredentialAsync";
                return result;
            }

            result.SecretSeed = secret_seed;
            result.Credential = credential_result;

            return result;

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            result.ResultMessage = e.Message;
            result.ResultCode = ResultStatusCodes.EXCEPTION;
            return result;
        }
    }

    //public async Task<string> LoginAsync()
    //{
    //    return "not yet implemented";
    //}

    //public async Task<string> LoginAsync()
    // returns result code and key seed
    public async Task<AuthnLoginResult> LoginAsync(string stored_credential)
    {
        var result = new AuthnLoginResult();
        // Now the magic happens so stuff can go wrong
        try
        {
            // Make sure the WebAuthn API is initialized (although that should happen almost immediately after startup)
            await _webAuthn.Init();

            // Get options from server
            //var options = await _httpClient.GetFromJsonAsync<AssertionOptions>(route, _jsonOptions);
            AssertionOptions options = _serverService.MakeAssertionOptions(stored_credential);

            if (options is null)
            {
                result.ResultCode = ResultStatusCodes.NOT_FOUND;
                result.ResultMessage = "No options received";
                return result;
            }

            if (options.Status != "ok")
            {
                result.ResultCode = ResultStatusCodes.EXCEPTION;
                result.ResultMessage = options.ErrorMessage;
                return result;
            }

            // Present options to user and get response (usernameless users will be asked by their authenticator, which credential they want to use to sign the challenge)
            var assertion = await _webAuthn.VerifyAsync(options);

            // Send response to server
            //return await (await _httpClient.PostAsJsonAsync($"{_routeUser}/{_routeLogin}", assertion, _jsonOptions)).Content.ReadAsStringAsync();
            var (assertion_result, sig) = await _serverService.MakeAssertionAsync(assertion, stored_credential);

            result.ResultCode = assertion_result;
            if (result.ResultCode == ResultStatusCodes.SUCCESS)
            {
                result.SecretSeed = sig;

            }
            else
            {
                result.ResultMessage = "Error in MakeAssertionAsync";
            }

            return result;

        }
        catch (Exception e)
        {
            result.ResultCode = ResultStatusCodes.UNKNOWN_ERROR;
            result.ResultMessage = e.Message;
            return result;
        }
    }

    public async Task<bool> IsWebAuthnSupportedAsync()
    {
        await _webAuthn.Init();
        return await _webAuthn.IsWebAuthnSupportedAsync();
    }
}
