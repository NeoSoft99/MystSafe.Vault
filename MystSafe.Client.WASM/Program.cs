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

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MystSafe.Client.App;
using Microsoft.AspNetCore.Components;
using IndexedDB.Blazor;
using MystSafe.Client.Engine;
using MudBlazor.Services;
using Fido2.BlazorWebAssembly;
using BlazorDownloadFile;
using MystSafe.Client.Base;
using MystSafe.Client.CryptoLicense;
using MystSafe.Shared.CryptoLicense;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton<BrowserInfoService, BrowserInfoService>();
builder.Services.AddSingleton<DrawerState, DrawerState>();
builder.Services.AddSingleton<EULAStatus, EULAStatus>();

builder.Services.AddSingleton<IIndexedDbFactory, IndexedDbFactory>();

builder.Services.AddSingleton<ChatsDrawerNotification, ChatsDrawerNotification>();
builder.Services.AddSingleton<SecretsDrawerNotification, SecretsDrawerNotification>();

builder.Services.AddSingleton<ChatNotification, ChatNotification>();
builder.Services.AddSingleton<SecretNotification, SecretNotification>();

builder.Services.AddSingleton<MainLayoutNotification, MainLayoutNotification>();
builder.Services.AddSingleton<UIModeNotification, UIModeNotification>();

builder.Services.AddMudServices();

// *** Passkey/biometric/webauthn/fido2 services
builder.Services.AddSingleton<IRuntimeFingerprintService, WASMFingerprintService>();
builder.Services.AddSingleton<LocalStorageEncryptionService>();
builder.Services.AddSingleton<InactivityTimerService>();
builder.Services.AddWebAuthn();
builder.Services.AddSingleton<IPassKeysService>(services =>
{
    var local_storage_encryptor = services.GetRequiredService<LocalStorageEncryptionService>();
    var webAuthn = services.GetRequiredService<WebAuthn>();
    var inactivityTimerService = services.GetRequiredService<InactivityTimerService>();

    var server_service = new WebAuthnService(
    webAuthn,
    local_storage_encryptor,
    builder.HostEnvironment.BaseAddress,
    inactivityTimerService);
    return server_service;
});
builder.Services.AddSingleton<AccessControlService>();
// ***

//builder.Services.AddSingleton<ApiClientService>();

builder.Services.AddSingleton(serviceProvider =>
{
    var backendUrl = serviceProvider.GetRequiredService<NavigationManager>().BaseUri;
    var logger = serviceProvider.GetRequiredService<ILogger<ApiClientService>>();
    return new ApiClientService(logger, backendUrl);
});

builder.Services.AddSingleton(serviceProvider =>
{
    var logger = serviceProvider.GetRequiredService<ILogger<LicenseRelayClientService>>();
    var apiClientService = serviceProvider.GetRequiredService<ApiClientService>();
    return new LicenseRelayClientService(logger, apiClientService.BackEndURL, apiClientService.APIClient);
});

builder.Services.AddSingleton(services =>
{
    var logger = services.GetRequiredService<ILogger<Wallet>>();
    var db_factory = services.GetRequiredService<IIndexedDbFactory>();
    var wallet_db = new WalletIndexedDb(db_factory);
    var relayClientService = services.GetRequiredService<LicenseRelayClientService>();
    var wallet = new Wallet(logger, relayClientService, wallet_db);
    return wallet;
});

builder.Services.AddSingleton(services =>
{
    //var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    //ClientSideLogger.InitializeLogger(loggerFactory);

    var backendUrl = services.GetRequiredService<NavigationManager>().BaseUri;
    var local_storage_encryptor = services.GetRequiredService<LocalStorageEncryptionService>();

    var access_control_service = services.GetRequiredService<AccessControlService>();

    var db_factory = services.GetRequiredService<IIndexedDbFactory>();
    var account_db = new AccountIndexedDb(db_factory, local_storage_encryptor);
    var contact_db = new ContactIndexedDb(db_factory, local_storage_encryptor);
    var chatin_db = new ChatInIndexedDb(db_factory, local_storage_encryptor);
    var chatout_db = new ChatOutIndexedDb(db_factory, local_storage_encryptor);
    var message_db = new MessageIndexedDb(db_factory, local_storage_encryptor);
    var secret_db = new SecretIndexedDb(db_factory, local_storage_encryptor);

    var logger = services.GetRequiredService<ILogger<SendProcessor>>();
    var apiClient = services.GetRequiredService<ApiClientService>();
    var wallet = services.GetRequiredService<Wallet>();

    var sendProcessor = new SendProcessor(
        backendUrl, account_db, contact_db, chatin_db, chatout_db, message_db, secret_db,
        access_control_service,
        logger,
        apiClient,
        wallet);

    return sendProcessor;
});

builder.Services.AddSingleton<AccountService>();

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.SetMinimumLevel(LogLevel.Debug);
});

builder.Services.AddSingleton(services =>
{
 
    var cp = services.GetRequiredService<SendProcessor>();

    var up = new UpdateProcessor(
        true,
        cp._Account_Db,
        cp._Contact_Db,
        cp._ChatIn_Db,
        cp._ChatOut_Db,
        cp._Message_Db,
        cp._Secret_Db,
        cp.Logger,
        cp._apiClientService);
    return up;
});



builder.Services.AddBlazorDownloadFile();
builder.Services.AddScoped<SecretExportService>();
builder.Services.AddSingleton<ClientVersionService>();

await builder.Build().RunAsync();

