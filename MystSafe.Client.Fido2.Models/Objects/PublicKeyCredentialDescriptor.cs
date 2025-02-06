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

#nullable enable

using System;
using System.Text.Json.Serialization;

namespace Fido2NetLib.Objects;

/// <summary>
/// This object contains the attributes that are specified by a caller when referring to a public key credential as an input parameter to the create() or get() methods. It mirrors the fields of the PublicKeyCredential object returned by the latter methods.
/// Lazy implementation of https://www.w3.org/TR/webauthn/#dictdef-publickeycredentialdescriptor
/// todo: Should add validation of values as specified in spec
/// </summary>
public sealed class PublicKeyCredentialDescriptor
{
    public PublicKeyCredentialDescriptor(byte[] id)
        : this(PublicKeyCredentialType.PublicKey, id, null) { }

    [JsonConstructor]
    public PublicKeyCredentialDescriptor(PublicKeyCredentialType type, byte[] id, AuthenticatorTransport[]? transports = null)
    {
        ArgumentNullException.ThrowIfNull(id);

        Type = type;
        Id = id;
        Transports = transports;
    }

    /// <summary>
    /// This member contains the type of the public key credential the caller is referring to.
    /// </summary>
    [JsonPropertyName("type")]
    public PublicKeyCredentialType Type { get; }

    /// <summary>
    /// This member contains the credential ID of the public key credential the caller is referring to.
    /// </summary>
    [JsonConverter(typeof(Base64UrlConverter))]
    [JsonPropertyName("id")]
    public byte[] Id { get; }

    /// <summary>
    /// This OPTIONAL member contains a hint as to how the client might communicate with the managing authenticator of the public key credential the caller is referring to.
    /// </summary>
    [JsonPropertyName("transports")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public AuthenticatorTransport[]? Transports { get; }
};
