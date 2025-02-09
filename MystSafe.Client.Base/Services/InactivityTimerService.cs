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
using Microsoft.AspNetCore.Components;

namespace MystSafe.Client.Base;

public class InactivityTimerService
{
    public string RecentPageURL { get; set; } = string.Empty;
    private readonly IJSRuntime _jsRuntime;
    private readonly DotNetObjectReference<InactivityTimerService> _objectReference;
    //private AccessControlService _accessControlService;
    //private readonly AccountService _accountService;
    private readonly NavigationManager _navigationManager;

    public InactivityTimerService(IJSRuntime jsRuntime, NavigationManager navigationmanager)
    {
        _jsRuntime = jsRuntime;
        _objectReference = DotNetObjectReference.Create(this);
        //_accessControlService = accessControlService;
        //_accountService = accountService;
        _navigationManager = navigationmanager;
    }

    public event EventHandler<EventArgs>? OnInactivityTimer;

    public string LockedPage { get; set; } = "/secret";
    public bool DrawerisOpen { get; set; }
    public int TimeOutSec { get; set; } = ClientConstants.DEFAULT_INACTIVITY_TIMEOUT_SECONDS;

    [JSInvokable]
    public async Task OnTimer()
    {
        LockedPage = RecentPageURL; //GetPageName();
        await Task.Yield();
        OnInactivityTimer?.Invoke(this, new EventArgs());
    }

    public async Task StartTimer()
    {
        await _jsRuntime.InvokeVoidAsync("activityDetector.startTimer", _objectReference, TimeOutSec * 1000);
    }

    public async Task StartTimer(int timeout_sec)
    {

        TimeOutSec = timeout_sec;
        await StartTimer();
    }

    
    public async Task ResetTimer()
    {
        await _jsRuntime.InvokeVoidAsync("activityDetector.resetInactivityTimer", TimeOutSec * 1000);
    }

    public async Task ResetTimer(int timeout_sec)
    {
        TimeOutSec = timeout_sec;
        await ResetTimer();
    }

    /*private string GetPageName()
    {
        var uri = new Uri(_navigationManager.Uri);

        // Assuming the page name is the last segment of the URL
        var segments = uri.Segments;
        if (segments.Length > 0)
        {
            //return segments.Last().Trim('/');
            return segments.Last();
        }

        return "/secret";
    }*/
}


