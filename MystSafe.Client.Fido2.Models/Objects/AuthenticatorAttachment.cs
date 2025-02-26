﻿// MystSafe is a secret vault with anonymous access and zero activity tracking protected by cryptocurrency-grade tech.
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

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Fido2NetLib.Objects;

/// <summary>
/// This enumeration’s values describe authenticators' attachment modalities. Relying Parties use this for two purposes:
/// to express a preferred authenticator attachment modality when calling navigator.credentials.create() to create a credential, and
/// to inform the client of the Relying Party's best belief about how to locate the managing authenticators of the credentials listed in allowCredentials when calling navigator.credentials.get().
/// </summary>
/// <remarks>
/// Note: An authenticator attachment modality selection option is available only in the [[Create]](origin, options, sameOriginWithAncestors) operation. The Relying Party may use it to, for example, ensure the user has a roaming credential for authenticating on another client device; or to specifically register a platform credential for easier reauthentication using a particular client device. The [[DiscoverFromExternalSource]](origin, options, sameOriginWithAncestors) operation has no authenticator attachment modality selection option, so the Relying Party SHOULD accept any of the user’s registered credentials. The client and user will then use whichever is available and convenient at the time.
/// https://www.w3.org/TR/webauthn-2/#enum-attachment
/// </remarks>
[JsonConverter(typeof(FidoEnumConverter<AuthenticatorAttachment>))]
public enum AuthenticatorAttachment
{
    /// <summary>
    /// This value indicates platform attachment
    /// </summary>
    [EnumMember(Value = "platform")]
    Platform,

    /// <summary>
    /// This value indicates cross-platform attachment.
    /// </summary>
    [EnumMember(Value = "cross-platform")]
    CrossPlatform
}
