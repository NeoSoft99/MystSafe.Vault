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
/// The CodeAccuracyDescriptor describes the relevant accuracy/complexity aspects of passcode user verification methods.
/// </summary>
/// <remarks>
/// <see href="https://fidoalliance.org/specs/fido-v2.0-rd-20180702/fido-metadata-statement-v2.0-rd-20180702.html#codeaccuracydescriptor-dictionary"/>
/// </remarks>
public sealed class CodeAccuracyDescriptor
{
    /// <summary>
    /// Gets or sets the numeric system base (radix) of the code, e.g.  10 in the case of decimal digits. 
    /// </summary>
    [JsonPropertyName("base"), Required]
    public ushort Base { get; set; }

    /// <summary>
    /// Gets or sets the minimum number of digits of the given base required for that code, e.g. 4 in the case of 4 digits.
    /// </summary>
    [JsonPropertyName("minLength"), Required]
    public ushort MinLength { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of false attempts before the authenticator will block this method (at least for some time).
    /// <para>Zero (0) means it will never block.</para>
    /// </summary>
    [JsonPropertyName("maxRetries")]
    public ushort MaxRetries { get; set; }

    /// <summary>
    /// Gets or sets the enforced minimum number of seconds wait time after blocking (e.g. due to forced reboot or similar). 
    /// <para>Zero (0) means this user verification method will be blocked, either permanently or until an alternative user verification method method succeeded.</para> 
    /// </summary>
    /// <remarks>
    /// All alternative user verification methods MUST be specified appropriately in the Metadata in <see cref="MetadataStatement.UserVerificationDetails"/>.
    /// </remarks>
    [JsonPropertyName("blockSlowdown")]
    public ushort BlockSlowdown { get; set; }
}
