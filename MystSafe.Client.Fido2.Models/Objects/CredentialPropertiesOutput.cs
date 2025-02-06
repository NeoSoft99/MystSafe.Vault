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
using System.Text.Json.Serialization;

namespace Fido2NetLib.Objects;

/// <summary>
/// This client registration extension facilitates reporting certain credential properties known by the client to the requesting WebAuthn Relying Party upon creation of a public key credential source as a result of a registration ceremony.
/// </summary>
public class CredentialPropertiesOutput
{
    /// <summary>
    /// This OPTIONAL property, known abstractly as the resident key credential property (i.e., client-side discoverable credential property), is a Boolean value indicating whether the PublicKeyCredential returned as a result of a registration ceremony is a client-side discoverable credential. If rk is true, the credential is a discoverable credential. if rk is false, the credential is a server-side credential. If rk is not present, it is not known whether the credential is a discoverable credential or a server-side credential.
    /// </summary>
    [JsonPropertyName("rk")]
    public bool Rk { get; set; }
}
