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

namespace MystSafe.Client.Base;

public class LocalAuthnTypes
{
    public const int RUNTIME_FINGERPRINT = 0;
    public const int PASSKEY = 1;
    public const int PASSWORD = 2;
    public const int PINCODE = 3;
}


public class AccessControlService
{
    private readonly IPassKeysService _passKeysService;
    private readonly IRuntimeFingerprintService _runtimeFingerprintService;
    private readonly LocalStorageEncryptionService _encryptor;
    //private readonly IInactivityTimerService _inactivityTimerService;

    public bool IsAppLocked { get; set; }

    public AccessControlService(
        IPassKeysService passKeysService,
        IRuntimeFingerprintService runtimeFingerprintService,
        LocalStorageEncryptionService encryptor)
    {
        _passKeysService = passKeysService;
        _runtimeFingerprintService = runtimeFingerprintService;
        _encryptor = encryptor;
    }

    //public async Task<bool> IsLoggedIn(Account account)
    public async Task<bool> IsLoggedIn(string account_id, int local_authn_type, string local_encryption_key, string passkey_credentials)
    {
        //if (account == null)
        //    return true;
        if (string.IsNullOrEmpty(account_id))
            return true; // throw new Exception("Incorrect account id");

        //if (account.LocalAuthnType == LocalAuthnTypes.RUNTIME_FINGERPRINT)
        if (local_authn_type == LocalAuthnTypes.RUNTIME_FINGERPRINT)
            return true;

        //if (account.LocalAuthnType == LocalAuthnTypes.PASSKEY)
        if (local_authn_type == LocalAuthnTypes.PASSKEY)
        {
            var result =
                //await _passKeysService.IsLoggedIn(account.Id, account.LocalEncryptionKey, account.PasskeyCredentials);
                await _passKeysService.IsLoggedIn(account_id, local_encryption_key, passkey_credentials);
            if (result)
            {
                //await _inactivityTimerService.ResetTimer();
            }

            return result;
        }

        return false;

    }

    //public async Task Logout(Account? account)
    public async Task Logout(string account_id, int local_authn_type)
    {
        IsAppLocked = true;

        //if (account == null)
        //    return;
        
        if (string.IsNullOrEmpty(account_id))
            return; 

        //if (account.LocalAuthnType == LocalAuthnTypes.PASSKEY)
        if (local_authn_type == LocalAuthnTypes.PASSKEY)
        {
            //_passKeysService.Logout(account.Id);
            _passKeysService.Logout(account_id);
        }

    }

    public async Task<AccountAuthnResult> EnablePasskey(string account_id)
    {
        try
        {
            var register_result = await _passKeysService.Register(account_id);
            if (register_result.ResultCode == ResultStatusCodes.SUCCESS)
            {
                //await _inactivityTimerService.ResetTimer();
            }

            //Console.WriteLine("EnablePasskey result: " + register_result.ResultCode);
            //Console.WriteLine("EnablePasskey message: " + register_result.ResultMessage);
            //Console.WriteLine("EnablePasskey PasskeyCredentials: " + register_result.PasskeyCredentials);
            return register_result;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception in EnablePasskey: " + ex.Message);
            return null;
        }
    }

    public async Task<AccountAuthnResult> DisablePasskey(string account_id)
    {
        var result = new AccountAuthnResult();
        //_passKeysService.Logout();
        var seed = await _runtimeFingerprintService.GetRuntimeFingerprintAsync();
        result.LocalEncryptionKey = _encryptor.ReencryptLocalEncryptionKey(account_id, seed);
        result.LocalAuthnType = LocalAuthnTypes.RUNTIME_FINGERPRINT;
        result.PasskeyCredentials = string.Empty;

        return result;

    }

    public async Task<AccountAuthnResult> InitializeNewAccount(string account_id)
    {
        var result = new AccountAuthnResult();
        result.LocalAuthnType = LocalAuthnTypes.RUNTIME_FINGERPRINT; // this is the default 0 but just in case
        var runtime_fingerprint = await _runtimeFingerprintService.GetRuntimeFingerprintAsync();
        result.LocalEncryptionKey = _encryptor.GenerateLocalEncryptionKey(account_id, runtime_fingerprint);
        return result;
    }

    public async Task InitializeExistingAccount(AccountAuthnResult account_authn_data)
    {
        //string seed;
        if (account_authn_data.LocalAuthnType == LocalAuthnTypes.RUNTIME_FINGERPRINT)
        {
            var seed = await _runtimeFingerprintService.GetRuntimeFingerprintAsync();
            _encryptor.RestoreLocalEncryptionKey(account_authn_data.LocalEncryptionKey, account_authn_data.AccountId,
                seed);
            return;
        }
        else if (account_authn_data.LocalAuthnType == LocalAuthnTypes.PASSKEY)
        {
            if (await _passKeysService.IsLoggedIn(account_authn_data.AccountId, account_authn_data.LocalEncryptionKey,
                    account_authn_data.PasskeyCredentials))
            {
                //seed = _passkey_service.SecretSeed;
                //await _inactivityTimerService.ResetTimer();
                return;
            }
            else
            {
                //result.ResultCode = ResultStatusCodes.AUTHENTICATION_FAILED;
                //result.ResultMessage = "Could not authenticate";
                //return result;
                throw new Exception("Could not authenticate");
            }
        }
        else
        {
            //result.ResultCode = ResultStatusCodes.WRONG_AUTHENTICATION_PARAMETER;
            //result.ResultMessage = "Authn type not supported";
            //return result;
            throw new Exception("Authentication type not supported");
        }
    }
}

