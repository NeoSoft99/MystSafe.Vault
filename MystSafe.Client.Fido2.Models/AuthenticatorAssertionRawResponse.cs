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

#nullable disable

using System.Text.Json.Serialization;

using Fido2NetLib.Objects;

namespace Fido2NetLib;

/// <summary>
/// Transport class for AssertionResponse
/// </summary>
public class AuthenticatorAssertionRawResponse
{
    [JsonConverter(typeof(Base64UrlConverter))]
    [JsonPropertyName("id")]
    public byte[] Id { get; set; }

    // might be wrong to base64url encode this...
    [JsonConverter(typeof(Base64UrlConverter))]
    [JsonPropertyName("rawId")]
    public byte[] RawId { get; set; }

    [JsonPropertyName("response")]
    public AssertionResponse Response { get; set; }

    [JsonPropertyName("type")]
    public PublicKeyCredentialType? Type { get; set; }

    [JsonPropertyName("extensions")]
    public AuthenticationExtensionsClientOutputs Extensions { get; set; }

    public class AssertionResponse
    {
        [JsonConverter(typeof(Base64UrlConverter))]
        [JsonPropertyName("authenticatorData")]
        public byte[] AuthenticatorData { get; set; }

        [JsonConverter(typeof(Base64UrlConverter))]
        [JsonPropertyName("signature")]
        public byte[] Signature { get; set; }

        [JsonConverter(typeof(Base64UrlConverter))]
        [JsonPropertyName("clientDataJSON")]
        public byte[] ClientDataJson { get; set; }
#nullable enable
        [JsonPropertyName("userHandle")]
        [JsonConverter(typeof(Base64UrlConverter))]
        public byte[]? UserHandle { get; set; }

        [JsonPropertyName("attestationObject")]
        [JsonConverter(typeof(Base64UrlConverter))]
        public byte[]? AttestationObject { get; set; }
    }
}
