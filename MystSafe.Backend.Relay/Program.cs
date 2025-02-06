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

using MystSafe.Backend.DB;
using Microsoft.AspNetCore.ResponseCompression;
using MystSafe.Backend.Relay;
using MystSafe.Shared.CryptoLicense;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

try
{
    var network_str = Environment.GetEnvironmentVariable(DatabaseService.NODE_NETWORK_CONFIG_NAME);
    Console.WriteLine("{0}: {1}", DatabaseService.NODE_NETWORK_CONFIG_NAME, network_str);

    var mongodb_connection_string =
        Environment.GetEnvironmentVariable(DatabaseService.MONGODB_CONNECTION_STRING_CONFIG_NAME);
    Console.WriteLine("{0}: {1}", DatabaseService.MONGODB_CONNECTION_STRING_CONFIG_NAME, mongodb_connection_string);

    var mongodb_database_name =
        Environment.GetEnvironmentVariable(DatabaseService.MONGODB_DATABASE_NAME_CONFIG_NAME);
    Console.WriteLine("{0}: {1}", DatabaseService.MONGODB_DATABASE_NAME_CONFIG_NAME, mongodb_database_name);
    
    const string LICENSE_ADMIN_RELAY_URL = "LICENSE_ADMIN_RELAY_URL";
    var license_admin_relay_url = Environment.GetEnvironmentVariable(LICENSE_ADMIN_RELAY_URL);
    Console.WriteLine("{0}: {1}", LICENSE_ADMIN_RELAY_URL, license_admin_relay_url);
    

    builder.Services.AddSingleton<DatabaseService>(services =>
    {
       
        var service = new DatabaseService(
            network_str,
            mongodb_connection_string,
            mongodb_database_name);
        return service;
    });
    
    builder.Services.AddSingleton<SecretBlockDb>();
    builder.Services.AddSingleton<ContactBlockDb>();
    builder.Services.AddSingleton<ChatBlockDb>();
    builder.Services.AddSingleton<MsgBlockDb>();
    
    builder.Services.AddSingleton<ILicenseRelayClientService>(serviceProvider =>
    {
        var logger = serviceProvider.GetRequiredService<ILogger<LicenseRelayClientService>>();
        var apiClient = new HttpClient();
        return new LicenseRelayClientService(logger, license_admin_relay_url, apiClient);
    });
    
    builder.Services.AddSingleton<BackendLicenseValidationService>();

    builder.Services.AddSingleton<BackendVersionService>();
    
    var APPLICATION_PORT = 0;
    var applicationPortString = Environment.GetEnvironmentVariable("APPLICATION_PORT");
    if (!int.TryParse(applicationPortString, out APPLICATION_PORT))
    {
        // Handle the case where the environment variable is not a valid integer
        //throw new Exception("No APPLICATION_PORT defined");
        APPLICATION_PORT = 80;
    }

    builder.WebHost.ConfigureKestrel(serverOptions => { serverOptions.ListenAnyIP(APPLICATION_PORT); });


// builder.Services.AddHttpsRedirection(options =>
// {
//     options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
//     options.HttpsPort = APPLICATION_PORT; // 443
// });

    Console.WriteLine("Initializing relay...");
    Console.WriteLine("APPLICATION_PORT: " + APPLICATION_PORT);

    builder.Services.AddControllers();

    builder.Services.AddResponseCompression(opts =>
    {
        opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
            new[] { "application/octet-stream" });
    });

    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(
            builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
    });


    var app = builder.Build();

// Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        // app.UseWebAssemblyDebugging();
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Error");
    }

//app.UseBlazorFrameworkFiles();

//Console.WriteLine($"Client App Version: {MystSafe.Common.Constants.CLIENT_APP_VERSION}");
    var versionService = app.Services.GetRequiredService<BackendVersionService>();
    Console.WriteLine($"Release Version: {versionService.GetReleaseVersion()}");

// app.UseStaticFiles(new StaticFileOptions
// {
//     OnPrepareResponse = ctx =>
//     {
//         ctx.Context.Response.Headers.Append("Cache-Control", $"no-cache, must-revalidate, version={versionService.GetReleaseVersion()}");
//     },
//     ContentTypeProvider = new FileExtensionContentTypeProvider()
// });
    app.UseCors();
    app.UseRouting();
    
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapGet("api", async context =>
        {
            await context.Response.WriteAsync("OK");
        });
    });


//#pragma warning disable ASP0014 // Suggest using top level route registrations
//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapGrpcService<ContactBlockNodeServiceImpl>().RequireCors("AllowAll");
//    endpoints.MapGrpcService<InitBlockNodeServiceImpl>().RequireCors("AllowAll");
//    endpoints.MapGrpcService<MsgBlockNodeServiceImpl>().RequireCors("AllowAll");
//    endpoints.MapGrpcService<SecretBlockNodeServiceImpl>().RequireCors("AllowAll");
//    endpoints.MapGrpcService<LicenseBlockNodeServiceImpl>().RequireCors("AllowAll");
//});
//#pragma warning restore ASP0014 // Suggest using top level route registrations

    app.MapControllers();
//app.MapFallbackToFile("index.html");

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Unhandled exception: {ex}");
    throw;
}
finally
{
    Console.WriteLine("App is shutting down...");
}
