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

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Fido2NetLib.Objects;

/// <summary>
/// This is a dictionary containing the PRF extension input values
/// </summary>
public sealed class AuthenticationExtensionsPRFInputs
{
    /// <summary>
    /// Inputs on which to evaluate PRF.
    /// https://w3c.github.io/webauthn/#dom-authenticationextensionsprfinputs-eval
    /// </summary>
    [JsonPropertyName("eval")]
    public AuthenticationExtensionsPRFValues Eval { get; set; }
    /// <summary>
    /// A record mapping base64url encoded credential IDs to PRF inputs to evaluate for that credential.
    /// https://w3c.github.io/webauthn/#dom-authenticationextensionsprfinputs-evalbycredential
    /// </summary>
    [JsonPropertyName("evalByCredential")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public KeyValuePair<string, AuthenticationExtensionsPRFValues>? EvalByCredential { get; set; }
}
