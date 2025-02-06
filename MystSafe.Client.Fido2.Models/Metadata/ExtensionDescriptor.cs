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
/// This descriptor contains an extension supported by the authenticator. 
/// </summary>
/// <remarks>
/// <see href="https://fidoalliance.org/specs/fido-v2.0-rd-20180702/fido-metadata-statement-v2.0-rd-20180702.html#extensiondescriptor-dictionary"/>
/// </remarks>
public class ExtensionDescriptor
{
    /// <summary>
    /// Gets or sets the identifier that identifies the extension.
    /// </summary>
    [JsonPropertyName("id"), Required]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the tag.
    /// <para>This field may be empty.</para>
    /// </summary>
    /// <remarks>
    /// The TAG of the extension if this was assigned. TAGs are assigned to extensions if they could appear in an assertion. 
    /// </remarks>
    [JsonPropertyName("tag")]
    public ushort Tag { get; set; }

    /// <summary>
    /// Gets or sets arbitrary data further describing the extension and/or data needed to correctly process the extension. 
    /// <para>This field may be empty.</para>
    /// </summary>
    /// <remarks>
    /// This field MAY be missing or it MAY be empty.
    /// </remarks>
    [JsonPropertyName("data")]
    public string Data { get; set; }

    /// <summary>
    /// Gets or sets a value indication whether an unknown extensions must be ignored (<c>false</c>) or must lead to an error (<c>true</c>) when the extension is to be processed by the FIDO Server, FIDO Client, ASM, or FIDO Authenticator. 
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///     <item>A value of false indicates that unknown extensions MUST be ignored.</item>
    ///     <item>A value of true indicates that unknown extensions MUST result in an error.</item>
    /// </list>
    /// </remarks>
    [JsonPropertyName("fail_if_unknown"), Required]
    public bool Fail_If_Unknown { get; set; }
}
