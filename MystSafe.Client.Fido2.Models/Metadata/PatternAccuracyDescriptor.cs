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

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Fido2NetLib;

/// <summary>
/// The PatternAccuracyDescriptor describes relevant accuracy/complexity aspects in the case that a pattern is used as the user verification method.
/// </summary>
/// <remarks>
/// <see href="https://fidoalliance.org/specs/fido-v2.0-rd-20180702/fido-metadata-statement-v2.0-rd-20180702.html#patternaccuracydescriptor-dictionary"/>
/// </remarks>
public sealed class PatternAccuracyDescriptor
{
    /// <summary>
    /// Gets or sets the number of possible patterns (having the minimum length) out of which exactly one would be the right one, i.e. 1/probability in the case of equal distribution.
    /// </summary>
    [JsonPropertyName("minComplexity"), Required]
    public ulong MinComplexity { get; set; }

    /// <summary>
    /// Gets or sets maximum number of false attempts before the authenticator will block authentication using this method (at least temporarily). 
    /// <para>Zero (0) means it will never block.</para>
    /// </summary>
    [JsonPropertyName("maxRetries")]
    public ushort MaxRetries { get; set; }

    /// <summary>
    /// Gets or sets the enforced minimum number of seconds wait time after blocking (due to forced reboot or similar mechanism).
    /// <para>Zero (0) means this user verification method will be blocked, either permanently or until an alternative user verification method method succeeded.</para>
    /// </summary>
    /// <remarks>
    /// All alternative user verification methods MUST be specified appropriately in the metadata under userVerificationDetails.
    /// </remarks>
    [JsonPropertyName("blockSlowdown")]
    public ushort BlockSlowdown { get; set; }
}
