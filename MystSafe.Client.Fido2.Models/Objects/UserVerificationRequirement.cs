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

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Fido2NetLib.Objects;

/// <summary>
/// A WebAuthn Relying Party may require user verification for some of its operations but not for others, 
/// and may use this type to express its needs.
/// https://www.w3.org/TR/webauthn-2/#enumdef-userverificationrequirement
/// </summary>
[JsonConverter(typeof(FidoEnumConverter<UserVerificationRequirement>))]
public enum UserVerificationRequirement
{
    /// <summary>
    /// This value indicates that the Relying Party requires user verification for the operation 
    /// and will fail the operation if the response does not have the UV flag set.
    /// </summary>
    [EnumMember(Value = "required")]
    Required,

    /// <summary>
    /// This value indicates that the Relying Party prefers user verification for the operation if possible, 
    /// but will not fail the operation if the response does not have the UV flag set.
    /// </summary>
    [EnumMember(Value = "preferred")]
    Preferred,

    /// <summary>
    /// This value indicates that the Relying Party does not want user verification employed during the operation 
    /// (e.g., in the interest of minimizing disruption to the user interaction flow).
    /// </summary>
    [EnumMember(Value = "discouraged")]
    Discouraged
}
