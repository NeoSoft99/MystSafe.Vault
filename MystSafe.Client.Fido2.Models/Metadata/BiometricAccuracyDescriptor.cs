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
/// The BiometricAccuracyDescriptor describes relevant accuracy/complexity aspects in the case of a biometric user verification method.
/// </summary>
/// <remarks>
/// <see href="https://fidoalliance.org/specs/fido-v2.0-rd-20180702/fido-metadata-statement-v2.0-rd-20180702.html#biometricaccuracydescriptor-dictionary"/>
/// </remarks>
public sealed class BiometricAccuracyDescriptor
{
    /// <summary>
    /// Gets or sets the false rejection rate.
    /// <para>For example a FRR of 10% would be encoded as 0.1.</para>
    /// </summary>
    /// <remarks>
    ///  [ISO19795-1] for a single template, i.e. the percentage of verification transactions with truthful claims of identity that are incorrectly denied. 
    /// </remarks>
    [JsonPropertyName("selfAttestedFRR")]
    public double SelfAttestedFRR { get; set; }

    /// <summary>
    /// Gets or sets the false acceptance rate.
    /// <para>For example a FAR of 0.002% would be encoded as 0.00002.</para>
    /// </summary>
    [JsonPropertyName("selfAttestedFAR")]
    public double SelfAttestedFAR { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of alternative templates from different fingers allowed.
    /// </summary>
    /// <remarks>
    /// For other modalities, multiple parts of the body that can be used interchangeably.
    /// For example: 3 if the user is allowed to enroll up to 3 different fingers to a fingerprint based authenticator. 
    /// </remarks>
    [JsonPropertyName("maxTemplates")]
    public ushort MaxTemplates { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of false attempts before the authenticator will block this method (at least for some time).
    /// <para>Zero (0) means it will never block.</para>
    /// </summary>
    [JsonPropertyName("maxRetries")]
    public ushort MaxRetries { get; set; }

    /// <summary>
    /// Gets or sets the enforced minimum number of seconds wait time after blocking (e.g. due to forced reboot or similar).
    /// <para>Zero (0) means that this user verification method will be blocked either permanently or until an alternative user verification method succeeded.</para>
    /// </summary>
    /// <remarks>
    /// All alternative user verification methods MUST be specified appropriately in the metadata in <see cref="MetadataStatement.UserVerificationDetails"/>.
    /// </remarks>
    [JsonPropertyName("blockSlowdown")]
    public ushort BlockSlowdown { get; set; }
}
