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

using System.Text.Json.Serialization;

namespace Fido2NetLib;

/// <summary>
/// A descriptor for a specific base user verification method as implemented by the authenticator.
/// </summary>
/// <remarks>
/// <see href="https://fidoalliance.org/specs/fido-v2.0-rd-20180702/fido-metadata-statement-v2.0-rd-20180702.html#verificationmethoddescriptor-dictionary"/>
/// </remarks>
public class VerificationMethodDescriptor
{
    /// <summary>
    /// Gets or sets a single USER_VERIFY constant, not a bit flag combination. 
    /// </summary>
    /// <remarks>
    /// This value MUST be non-zero.
    /// </remarks>
    [JsonPropertyName("userVerificationMethod")]
    public string UserVerificationMethod { get; set; }

    /// <summary>
    /// Gets or sets a may optionally be used in the case of method USER_VERIFY_PASSCODE.
    /// </summary>
    [JsonPropertyName("caDesc")]
    public CodeAccuracyDescriptor CaDesc { get; set; }

    /// <summary>
    /// Gets or sets a may optionally be used in the case of method USER_VERIFY_FINGERPRINT, USER_VERIFY_VOICEPRINT, USER_VERIFY_FACEPRINT, USER_VERIFY_EYEPRINT, or USER_VERIFY_HANDPRINT.
    /// </summary>
    [JsonPropertyName("baDesc")]
    public BiometricAccuracyDescriptor BaDesc { get; set; }

    /// <summary>
    /// Gets or sets a may optionally be used in case of method USER_VERIFY_PATTERN.
    /// </summary>
    [JsonPropertyName("paDesc")]
    public PatternAccuracyDescriptor PaDesc { get; set; }
}
