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

namespace MystSafe.Client.Base;

public class AuthnRegisterResult
{
    public int ResultCode { get; set; }
    public string? Credential { get; set; }
    public string? SecretSeed { get; set; }
    public string? ResultMessage { get; set; }
}

public class AuthnLoginResult
{
    public int ResultCode { get; set; }
    public string? SecretSeed { get; set; }
    public string? ResultMessage { get; set; }
}


public class WebAuthnService: IPassKeysService
{
    private readonly WebAuthnUser _user;
    //private readonly AccountService _accountService;
    private readonly LocalStorageEncryptionService _local_storage_encryptor;
    private readonly InactivityTimerService _inactivityTimerService;

    private bool _IsAsserted = false;

    //private string _secret_seed;

    //public string SecretSeed
    //{
    //    get { return _secret_seed; }
    //}

    public WebAuthnService(
        //AccountService accountService,
        WebAuthn webAuthn,
        LocalStorageEncryptionService localStorageEncryptionService,
        string base_address,
        InactivityTimerService inactivityTimerService)
	{
        //_accountService = accountService;
        var server = new WebAuthnServer(base_address);
        _user = new WebAuthnUser(webAuthn, server);
        _local_storage_encryptor = localStorageEncryptionService;
        _inactivityTimerService = inactivityTimerService;
    }

    public async Task<AccountAuthnResult> Register(string account_id)
    {
        await _inactivityTimerService.ResetTimer();
        _IsAsserted = false;
        var result = new AccountAuthnResult();

        var register_result = await _user.RegisterAsync();
        result.ResultCode = register_result.ResultCode;
        result.AccountId = account_id;


        if (register_result.ResultCode == ResultStatusCodes.SUCCESS)
        {
            result.LocalAuthnType = LocalAuthnTypes.PASSKEY;
            result.PasskeyCredentials = register_result.Credential;
            result.LocalEncryptionKey = _local_storage_encryptor.ReencryptLocalEncryptionKey(account_id, register_result.SecretSeed);

            _IsAsserted = true;
        }
        else
        {
            result.ResultMessage = register_result.ResultMessage;
            _IsAsserted = true;
        }
        return result;
    }

    // public async Task<bool> IsLoggedIn(AccountAuthnResult account_authn_data)
    public async Task<bool> IsLoggedIn(string account_id, string encrypted_account_key, string stored_credential)
    {
        await _inactivityTimerService.ResetTimer();

        if (!_IsAsserted)
        {
            var login_result = await _user.LoginAsync(stored_credential);

            _IsAsserted = login_result.ResultCode == ResultStatusCodes.SUCCESS;

            if (_IsAsserted)
            {
                
                _local_storage_encryptor.RestoreLocalEncryptionKey(encrypted_account_key, account_id, login_result.SecretSeed);

            }
        }

        return _IsAsserted;
    }

    public void Logout(string account_id)
    {
        _IsAsserted = false;
        _local_storage_encryptor.ResetLocalEncryptionKey(account_id);
    }

    
}


