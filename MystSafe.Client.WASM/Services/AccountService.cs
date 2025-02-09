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
using Microsoft.AspNetCore.Components;
using MystSafe.Client.Engine;
using MystSafe.Client.Base;
using MystSafe.Client.CryptoLicense;

namespace MystSafe.Client.App;

public class AccountService
{
    public Account? CurrentAccount { get; set; }
    


    private readonly NavigationManager _NavigationManager;
    private readonly SendProcessor _SendProcessor;
    //private readonly Wallet _wallet;
    private readonly InactivityTimerService _inactivityTimerService;
    private readonly AccessControlService _accessControlService;

    private bool load_in_progress = false;

    // the list addresses that are allowed to see License page
    private readonly List<string> license_allowed_prod_accounts = new List<string> { "S21wfFoxRqRxHR5dZ9vRYdEGN51pvvfELGkJkPhqyAeTNSimrhFDhMFwTY5bpA6KepuoNrtLcEVsHkYzXYwy5fjsQQ2Y" };

    // the list addresses that are allowed to see Master Key page
    private readonly List<string> master_key_allowed_prod_accounts = new List<string> { "S21wfFoxRqRxHR5dZ9vRYdEGN51pvvfELGkJkPhqyAeTNSimrhFDhMFwTY5bpA6KepuoNrtLcEVsHkYzXYwy5fjsQQ2Y" };

    public AccountService(
        NavigationManager navigationManager,
        SendProcessor sendProcessor,
        InactivityTimerService inactivityTimerService,
        AccessControlService accessControlService)
        //Wallet wallet)
    {
        _SendProcessor = sendProcessor;
        _NavigationManager = navigationManager;
        _inactivityTimerService = inactivityTimerService;
        _accessControlService = accessControlService;
        //_wallet = wallet;
    }

    // indicates whether the app is active or idle (no open account)
    public bool HasActiveAccount
    {
        get { return CurrentAccount != null; }
    }
    
    
    /*
    public string GetLicenseDescription()
    {
        if (!HasActiveAccount)
        {
            return string.Empty;
        }

        if (string.IsNullOrEmpty(CurrentAccount.LicensePrivateKey))
        {
            return "Free";
        }
        else
        {
            return "Premium until " + UnixDateTime.ToDateTime(CurrentAccount.LicenseExpirationDate).ToLongDateString();
        }

    }
    */

    
    public bool CanAccessTestingPage()
    {
        var prod_domain = _SendProcessor.BackEndURL.Contains("app.mystsafe.com");

        if (prod_domain)
            return false;

        return true;
    }
    

    // return true if no page switch is required, or false if navigated to a different page
    public async Task<bool> PageSelector(PageTypes currentPage)
    {
        if (load_in_progress)
            return true;

        load_in_progress = true;
        try
        {
            _inactivityTimerService.RecentPageURL = PageUrls.TypeToURL(currentPage);
            if (!HasActiveAccount)
            {
                // try to load acount from database - in case the current page is refreshed
                var result = await _SendProcessor.GetRecentAccountAsync();
                if (result.ResultCode == ResultStatusCodes.SUCCESS)
                {
                    CurrentAccount = result.ResultAccount;
                    //await _inactivityTimerService.ResetTimer(CurrentAccount.LockTimeoutSec);
                }
            }

            if (!HasActiveAccount)
            {
  
                if (currentPage == PageTypes.CREATE_ACCOUNT || currentPage == PageTypes.INSTANT_SHARE)
                {
                    return true;
                }
                else
                if (currentPage == PageTypes.TESTING)
                {
                    return true;
                }
                else
                {
                    // if no account was found in the browser database, create a new one
                    _NavigationManager.NavigateTo(PageUrls.CREATE_ACCOUNT);
                    return false;
                }
            }

         
            await _inactivityTimerService.ResetTimer(CurrentAccount.LockTimeoutSec);

            // process redirections
            switch (currentPage)
            {
                case PageTypes.CHAT:
                    if (CurrentAccount.CurrentContact == null)
                    {
                        _NavigationManager.NavigateTo(PageUrls.NEW_CHAT);
                        return false;
                    }
                    break;
                case PageTypes.SECRET:
                    if (CurrentAccount.CurrentSecret == null)
                    {
                        _NavigationManager.NavigateTo(PageUrls.NEW_SECRET);
                        return false;
                    }
                    break;
                case PageTypes.FOLDER:
                    if (CurrentAccount.CurrentSecret == null ||
                        CurrentAccount.CurrentSecret.Data.SecretType != SecretTypes.Folder)
                    {
                        _NavigationManager.NavigateTo(PageUrls.NEW_SECRET);
                        return false;
                    }
                    break;
                case PageTypes.MAIN_LAYOUT:
                    /*if (CurrentAccount.Secrets.Count > 0)
                    {
                        _NavigationManager.NavigateTo(PageUrls.SECRET);
                        return false;
                    }
                    else
                    {
                        _NavigationManager.NavigateTo(PageUrls.CHAT);
                        return false;

                    }
                    */
                    _NavigationManager.NavigateTo(PageUrls.SECRET);
                    return false;

            }

            // process authentication
            switch (currentPage)
            {
                case PageTypes.CHAT:
                case PageTypes.NEW_CHAT:
                case PageTypes.SECRET:
                case PageTypes.NEW_SECRET:
                case PageTypes.FOLDER:
                case PageTypes.NEW_FOLDER:
                case PageTypes.DRAWER:
                case PageTypes.MANAGE_ACCOUNT:
                    if (!await _accessControlService.IsLoggedIn(CurrentAccount.Id, CurrentAccount.LocalAuthnType, CurrentAccount.LocalEncryptionKey, CurrentAccount.PasskeyCredentials))
                    {
                        _NavigationManager.NavigateTo(PageUrls.LOCKED);
                        return false;
                    }
                    break;
                case PageTypes.TESTING:
                    return CanAccessTestingPage();
            }

            return true;
        }
        finally
        {
            load_in_progress = false;
        }
    }
}

