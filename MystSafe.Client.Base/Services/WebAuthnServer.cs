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

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Fido2NetLib;
using Fido2NetLib.Objects;

namespace MystSafe.Client.Base;

public class StoredAuthnCredentials
{
    public byte[]? Id { get; set; }

    public byte[]? PublicKey { get; set; }

    public PublicKeyCredentialDescriptor Descriptor
    {
        get
        {
            return Id is not null ? new PublicKeyCredentialDescriptor(Id) : throw new Exception("Id is not found");
        }
    }

    // Serialize the byte[] fields to a Base64 string
    public string SerializeToBase64()
    {
        if (Id is null || PublicKey is null)
            throw new Exception("Credentials not found");
        return Convert.ToBase64String(Id) + ":" + Convert.ToBase64String(PublicKey);
    }

    // Deserialize the Base64 string back to byte[] fields
    public static StoredAuthnCredentials DeserializeFromBase64(string base64String)
    {
        var parts = base64String.Split(':');
        if (parts.Length != 2)
            throw new ArgumentException("The base64String is not in the expected format.");

        return new StoredAuthnCredentials
        {
            Id = Convert.FromBase64String(parts[0]),
            PublicKey = Convert.FromBase64String(parts[1])
        };
    }

}

public class WebAuthnServer
{
    //private const string _routeUser = "api/user";
    //private const string _routeCredOptions = "credential-options";
    //private const string _routeRegister = "credential";
    //private const string _routeAssertionOpts = "assertion-options";
    //private const string _routeLogin = "assertion";

    //private readonly JsonSerializerOptions _jsonOptions = new FidoBlazorSerializerContext().Options;
    
    //private readonly WebAuthn _webAuthn;

    private readonly Fido2Configuration _fido2configuration;

    private readonly Fido2NetLib.Fido2 _fido2;

    //private readonly byte[] _challenge;

    //private Fido2User? _user;

    private AssertionOptions? _assertion_options;

    //private StoredCredential? _stored_credential;
    //private StoredAuthnCredentials? _stored_credential;
    //private string? _stored_credential;

    //public string? StoredCredential {  get { return _stored_credential; } }

    //private readonly IMetadataService? _metadataService;

    public WebAuthnServer(string BaseAddress)
    {
        //_stored_credential = storedCredential;

        Uri baseUri = new Uri(BaseAddress);

        Console.WriteLine("BaseAddress: " + BaseAddress);
        Console.WriteLine("hostName: " + baseUri.Host);

        _fido2configuration = new Fido2Configuration();

        _fido2configuration.ServerDomain = baseUri.Host; //"local.mystsafe.com"; //"node0.docker.cryptachat.com";//"192.168.1.112/";//origin.Host;
        _fido2configuration.ServerName = "FIDO2 Server";
        _fido2configuration.Origins = new HashSet<string> { BaseAddress };// new HashSet<string> { "https://local.mystsafe.com" };// new HashSet<string> { "http://node0.docker.cryptachat.com:5148" }; //new HashSet<string> { "http://192.168.1.112:5148" };//{ "http://localhost:5148" };//new HashSet<string> { origin.AbsoluteUri };
        _fido2configuration.TimestampDriftTolerance = 1000;

        //_metadataService = null;

         _fido2 = new Fido2NetLib.Fido2(_fido2configuration);

        //_challenge = RandomNumberGenerator.GetBytes(_fido2configuration.ChallengeSize);


    }

    /// <summary>
    /// Creates options to create a new credential for a user.
    /// </summary>
    /// <param name="username">(optional) The user's internal identifier. Omit for usernameless account.</param>
    /// <param name="displayName">(optional as query) Name for display purposes.</param>
    /// <returns>A new <see cref="CredentialCreateOptions"/>. Contains an error message if .Status is "error".</returns>
    public (CredentialCreateOptions, string?) GetCredentialOptions()
    {
        try
        {
                var created = DateTime.UtcNow;
                    // More precise generated name for less collisions in _pendingCredentials
                var username = $"(MystSafe user created {created})";
    
                //var key = Convert.ToBase64String(Encoding.UTF8.GetBytes(username));

            // generate a random user id by concatenating the random seed with username and hashing the result;
            // this can be used as a seed for pseudo random AES256 key;
            // we don't store it anywhere but it will be returned by the authentcating device as UserHandle.
            var _random_seed = RandomNumberGenerator.GetBytes(16); // 256 bit
            var concat = _random_seed.Concat(Encoding.UTF8.GetBytes(username)).ToArray();
            var user_id = SHA256.HashData(concat);

            // 1. Get user from DB by username (in our example, auto create missing users)
            Fido2User user = new Fido2User
            {
                DisplayName = null,
                Name = username,
                Id = user_id //Encoding.UTF8.GetBytes(username) // byte representation of userID is required
            };

            //Console.WriteLine("GetCredentialOptions() user id: " + Convert.ToBase64String(user.Id));

            // 2. Get user existing keys by username
            var existingKeys = new List<PublicKeyCredentialDescriptor>(); 

            // 3. Build authenticator selection
            var authenticatorSelection = AuthenticatorSelection.Default;


            var options = _fido2.RequestNewCredential(
                user,
                existingKeys,
                authenticatorSelection,
                AttestationConveyancePreference.None,//attestationType ?? AttestationConveyancePreference.None,
                new AuthenticationExtensionsClientInputs
                {
                    Extensions = true,
                    UserVerificationMethod = true,
                    CredProps = true,
                    DevicePubKey = new AuthenticationExtensionsDevicePublicKeyInputs
                    {
                        //Attestation = attestationType?.ToString() ?? AttestationConveyancePreference.None.ToString()
                        Attestation = AttestationConveyancePreference.None.ToString()
                    },
                }
            );

            // 5. Temporarily store options, session/in-memory cache/redis/db
            //_pendingCredentials[key] = options;

            // 6. return options to client
            return (options, Convert.ToBase64String(user.Id));
        }
        catch (Exception e)
        {
            return (new CredentialCreateOptions { Status = "error", ErrorMessage = e.Message }, null);
        }
    }

    ///// <summary>
    ///// Returns CredentialCreateOptions including a challenge to be sent to the browser/authr to create new credentials
    ///// </summary>
    ///// <returns></returns>
    ///// <param name="attestationPreference">This member is intended for use by Relying Parties that wish to express their preference for attestation conveyance. The default is none.</param>
    ///// <param name="excludeCredentials">Recommended. This member is intended for use by Relying Parties that wish to limit the creation of multiple credentials for the same account on a single authenticator.The client is requested to return an error if the new credential would be created on an authenticator that also contains one of the credentials enumerated in this parameter.</param>
    //private CredentialCreateOptions RequestNewCredentialEx(
    //    Fido2User user,
    //    List<PublicKeyCredentialDescriptor> excludeCredentials,
    //    AuthenticatorSelection authenticatorSelection,
    //    AttestationConveyancePreference attestationPreference,
    //    AuthenticationExtensionsClientInputs? extensions = null)
    //{
    //    //byte[] challenge = RandomNumberGenerator.GetBytes(_config.ChallengeSize);

    //    return CredentialCreateOptions.Create(_fido2configuration, _challenge, user, authenticatorSelection, attestationPreference, excludeCredentials, extensions);
    //}

    /// <summary>
    /// Creates a new credential for a user.
    /// </summary>
    /// <param name="attestationResponse"></param>
    /// <returns>(result code, a string containing user handle or error message)</returns>
    public async Task<(int, string)> CreateCredentialAsync(CredentialCreateOptions options, AuthenticatorAttestationRawResponse attestationResponse)
    {
        try
        {
            //attestationResponse.Response.AttestationObject

            // 2. Create callback so that lib can verify credential id is unique to this user

            // 3. Verify and make the credentials
            var result = await _fido2.MakeNewCredentialAsync(attestationResponse, options, CredentialIdUniqueToUserAsync);
            //var result = await MakeNewCredentialAsyncEx(attestationResponse, options, CredentialIdUniqueToUserAsync);

            if (result.Status is "error" || result.Result is null)
            {
                return (-1, result.ErrorMessage ?? string.Empty);
            }

            //// 4. Store the credentials in db
            //_stored_credential = new StoredCredential
            //{
            //    //AttestationFormat = result.Result.AttestationFormat,
            //    Id = result.Result.Id,
            //    Descriptor = new PublicKeyCredentialDescriptor(result.Result.Id),
            //    PublicKey = result.Result.PublicKey,

            //    //UserHandle = result.Result.User.Id, // we don't store it
            //    //UserHandle = new byte[] { 0 },

            //    //SignCount = result.Result.SignCount,
            //    //RegDate = DateTimeOffset.UtcNow,
            //    //AaGuid = result.Result.AaGuid,

            //    //DevicePublicKeys = new List<byte[]> { result.Result.DevicePublicKey },
            //    //DevicePublicKeys = new List<byte[]> { },

            //    //Transports = result.Result.Transports,
            //    //IsBackupEligible = result.Result.IsBackupEligible,
            //    //IsBackedUp = result.Result.IsBackedUp,
            //    //AttestationObject = result.Result.AttestationObject,
            //    //AttestationClientDataJSON = result.Result.AttestationClientDataJson,
            //};

            var _stored_credential = new StoredAuthnCredentials
            {
                //AttestationFormat = result.Result.AttestationFormat,
                Id = result.Result.Id,
                PublicKey = result.Result.PublicKey
            }.SerializeToBase64();

            // 5. return OK to client

            //return (0, Convert.ToBase64String(result.Result.User.Id));
            return (0, _stored_credential);
        }
        catch (Exception e)
        {
            return (-10, e.Message);
        }
    }

    ///// <summary>
    ///// Verifies the response from the browser/authr after creating new credentials
    ///// </summary>
    ///// <param name="attestationResponse"></param>
    ///// <param name="origChallenge"></param>
    ///// <param name="isCredentialIdUniqueToUser"></param>
    ///// <returns></returns>
    //public async Task<CredentialMakeResult> MakeNewCredentialAsyncEx(
    //    AuthenticatorAttestationRawResponse attestationResponse,
    //    CredentialCreateOptions origChallenge,
    //    IsCredentialIdUniqueToUserAsyncDelegate isCredentialIdUniqueToUser)
    //{
    //    var parsedResponse = AuthenticatorAttestationResponse.Parse(attestationResponse);
    //    var success = await parsedResponse.VerifyAsync(origChallenge, _fido2configuration, isCredentialIdUniqueToUser, _metadataService);
    //    //string sig = parsedResponse.AttestationObject.AttStmt["sig"].ToString();

    //    // todo: Set Errormessage etc.
    //    return new CredentialMakeResult(
    //        status: "ok",
    //        errorMessage: string.Empty,
    //        //errorMessage: sig,
    //        result: success
    //    );
    //}


    public AssertionOptions MakeAssertionOptions(string stored_credential)
    {
        try
        {
            var existingKeys = new List<PublicKeyCredentialDescriptor>();
     
            if (stored_credential != null)
                existingKeys = new List<PublicKeyCredentialDescriptor>() { StoredAuthnCredentials.DeserializeFromBase64(stored_credential).Descriptor };

            var exts = new AuthenticationExtensionsClientInputs
            {
                UserVerificationMethod = true,
                Extensions = true,
                DevicePubKey = new AuthenticationExtensionsDevicePublicKeyInputs()
            };

            // 2. Create options (usernameless users will be prompted by their device to select a credential from their own list)
            _assertion_options = _fido2.GetAssertionOptions(
            //_assertion_options = GetAssertionOptionsEx(
                existingKeys,
                UserVerificationRequirement.Discouraged, 
                exts);

            // 4. Temporarily store options, session/in-memory cache/redis/db
            //_pendingAssertions[new string(options.Challenge.Select(b => (char)b).ToArray())] = options;

            // 5. return options to client
            return _assertion_options;
        }
        catch (Exception e)
        {
            return new AssertionOptions { Status = "error", ErrorMessage = e.Message };
        }
    }

    ///// <summary>
    ///// Returns AssertionOptions including a challenge to the browser/authr to assert existing credentials and authenticate a user.
    ///// </summary>
    ///// <returns></returns>
    //private AssertionOptions GetAssertionOptionsEx(
    //    IEnumerable<PublicKeyCredentialDescriptor> allowedCredentials,
    //    UserVerificationRequirement? userVerification,
    //    AuthenticationExtensionsClientInputs? extensions = null)
    //{
    //    return AssertionOptions.Create(_fido2configuration, _challenge, allowedCredentials, userVerification, extensions);
    //}

    /// <summary>
    /// Verifies an assertion response from a client, generating a new JWT for the user.
    /// </summary>
    /// <param name="clientResponse">The client's authenticator's response to the challenge.</param>
    /// <returns>
    /// tuple that contains (result code, signature of the challenge)
    /// Example successful response:
    /// (0, "XXXXX")
    /// Example error response:
    /// (-1, "Error: Invalid assertion")
    /// </returns>
    public async Task<(int, string)> MakeAssertionAsync(AuthenticatorAssertionRawResponse clientResponse, string stored_credential)
    {
        try
        {


            // 1. Get the assertion options we sent the client remove them from memory so they can't be used again
            var response = JsonSerializer.Deserialize<AuthenticatorResponse>(clientResponse.Response.ClientDataJson);
            if (response is null)
            {
                return (-1, "Error: Could not deserialize client data");
            }

            // *** extra checks
            //if (_stored_credential is null)
            //    return (-2, "Error: no stored credentials found");

            var user_id = Convert.ToBase64String(clientResponse.Id); //Encoding.UTF8.GetString(clientResponse.Id); //Encoding.UTF8.GetString(creds.UserHandle)
            var original_user_id = Convert.ToBase64String(StoredAuthnCredentials.DeserializeFromBase64(stored_credential).Id);

            if (user_id != original_user_id)
                return (-3, $"Error: Id does not match the original Id: {user_id} <> {original_user_id}");

            var user_handle = Convert.ToBase64String(clientResponse.Response.UserHandle);
            //var original_user_handle = Convert.ToBase64String(_stored_credential.UserHandle);

            //Console.WriteLine("MakeAssertionAsync() user_handle: " + user_handle);
            //Console.WriteLine("MakeAssertionAsync() original_user_handle: " + original_user_handle);

            //if (user_handle != original_user_handle)
            //    return (-30, $"Error: User handle does not match the original Id: {user_id} <> {original_user_id}");

            // ***

            //var key = new string(response.Challenge.Select(b => (char)b).ToArray());

            // 2. Get registered credential from database
            var creds = StoredAuthnCredentials.DeserializeFromBase64(stored_credential) ?? throw new Exception("Unknown credentials");

            // 3. Make the assertion
            //var res = await _fido2.MakeAssertionAsync(
            //var (res, sig) = await MakeAssertionAsyncEx(
            //    clientResponse,
            //    _assertion_options ?? throw new Exception("Challenge not found, please get a new one"),
            //    creds.PublicKey,
            //    creds.DevicePublicKeys,
            //    creds.SignCount,
            //    UserHandleOwnerOfCredentialIdAsync);

            var (res, sig) = await MakeAssertionAsyncEx(
                clientResponse,
                _assertion_options ?? throw new Exception("Challenge not found, please get a new one"),
                creds.PublicKey,
                new List<byte[]> { }, //creds.DevicePublicKeys,
                0, //creds.SignCount,
                UserHandleOwnerOfCredentialIdAsync);

            // 4. Store the updated counter
            if (res.Status is "ok")
            {
                //_demoStorage.UpdateCounter(res.CredentialId, res.SignCount);
                //_stored_credential.SignCount++;
                //if (res.DevicePublicKey is not null)
                //{
                //    _stored_credential.DevicePublicKeys.Add(res.DevicePublicKey);
                //}
            }
            else
            {
                return (-4, $"Error: {res.ErrorMessage}");
            }

            // 5. return result to client
            //var handler = new JwtSecurityTokenHandler();
            //var token = handler.CreateEncodedJwt(
            //    HttpContext.Request.Host.Host,
            //    HttpContext.Request.Headers.Referer,
            //    new ClaimsIdentity(new Claim[] { new(ClaimTypes.Actor, Encoding.UTF8.GetString(creds.UserHandle)) }),
            //    DateTime.Now.Subtract(TimeSpan.FromMinutes(1)),
            //    DateTime.Now.AddDays(1),
            //    DateTime.Now,
            //    _signingCredentials,
            //    null);

            //if (token is null)
            //{
            //    return "Error: Token couldn't be created";
            //}


            return (0, sig); //$"Bearer " + user_id;
        }
        catch (Exception e)
        {
            return (-5, $"Error: {e.Message}");
        }
    }

    /// <summary>
    /// Verifies the assertion response from the browser/authr to assert existing credentials and authenticate a user.
    /// </summary>
    /// <returns>(VerifyAssertionResult, signature of the challenge)</returns>
    public async Task<(VerifyAssertionResult, string)> MakeAssertionAsyncEx(
        AuthenticatorAssertionRawResponse assertionResponse,
        AssertionOptions originalOptions,
        byte[] storedPublicKey,
        List<byte[]> storedDevicePublicKeys,
        uint storedSignatureCounter,
        IsUserHandleOwnerOfCredentialIdAsync isUserHandleOwnerOfCredentialIdCallback,
        CancellationToken cancellationToken = default)
    {
        var parsedResponse = AuthenticatorAssertionResponse.Parse(assertionResponse);

        var result = await parsedResponse.VerifyAsync(originalOptions,
                                                      _fido2configuration,
                                                      storedPublicKey,
                                                      storedDevicePublicKeys,
                                                      storedSignatureCounter,
                                                      isUserHandleOwnerOfCredentialIdCallback,
                                                      null,
                                                      cancellationToken);
        //var sig = Convert.ToBase64String( parsedResponse.Signature);
        var sig = Convert.ToBase64String(parsedResponse.UserHandle ?? new byte[] { });
        return (result, sig);
    }

    private static async Task<bool> UserHandleOwnerOfCredentialIdAsync(IsUserHandleOwnerOfCredentialIdParams args, CancellationToken cancellationToken)
    {
        return true;
    }

    private static async Task<bool> CredentialIdUniqueToUserAsync(IsCredentialIdUniqueToUserParams args, CancellationToken cancellationToken)
    {
        //var users = await _demoStorage.GetUsersByCredentialIdAsync(args.CredentialId, cancellationToken);
        return true; //users.Count <= 0;
    }


}
