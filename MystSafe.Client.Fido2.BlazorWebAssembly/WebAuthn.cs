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

namespace Fido2.BlazorWebAssembly;
using Fido2NetLib;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

/// <summary>
/// Module for accessing the browser's WebAuthn API.
/// </summary>
public class WebAuthn
{
    private IJSObjectReference _jsModule = null!;
    private readonly Task _initializer;

    public WebAuthn(IJSRuntime js)
    {
        _initializer = Task.Run(async () =>
            _jsModule = await js.InvokeAsync<IJSObjectReference>("import", "./_content/MystSafe.Client.Fido2.BlazorWebAssembly/js/WebAuthn.js"));
    }

    /// <summary>
    /// Wait for this to make sure this module is initialized.
    /// </summary>
    /// <returns></returns>
    public Task Init() => _initializer;

    /// <summary>
    /// Whether or not this browser supports WebAuthn.
    /// </summary>
    /// <returns></returns>
    public async Task<bool> IsWebAuthnSupportedAsync() => await _jsModule.InvokeAsync<bool>("isWebAuthnPossible");

    /// <summary>
    /// Creates a new credential.
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public async Task<AuthenticatorAttestationRawResponse> CreateCredsAsync(CredentialCreateOptions options) =>
        await _jsModule.InvokeAsync<AuthenticatorAttestationRawResponse>("createCreds", options);

    /// <summary>
    /// Verifies a credential for login.
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public async Task<AuthenticatorAssertionRawResponse> VerifyAsync(AssertionOptions options) =>
        await _jsModule.InvokeAsync<AuthenticatorAssertionRawResponse>("verify", options);
}

public static class DependencyInjection
{
    /// <summary>
    /// Adds the <see cref="WebAuthn"/> service to the DI container.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddWebAuthn(this IServiceCollection services) =>
        services.AddSingleton<WebAuthn>();
}
