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

namespace Fido2NetLib;

/// <summary>
/// This descriptor contains description in alternative languages.
/// </summary>
/// <remarks>
/// <see href="https://fidoalliance.org/specs/fido-v2.0-rd-20180702/fido-metadata-statement-v2.0-rd-20180702.html#alternativedescriptions-dictionary"/>
/// </remarks>
public class AlternativeDescriptions
{
    /// <summary>
    /// Gets or sets alternative descriptions of the authenticator.
    /// <para>
    /// Contains IETF language codes as key (e.g. "ru-RU", "de", "fr-FR") and a localized description as value.
    /// </para>
    /// </summary>
    /// <remarks>
    /// Contains IETF language codes, defined by a primary language subtag, 
    /// followed by a region subtag based on a two-letter country code from [ISO3166] alpha-2 (usually written in upper case).
    /// <para>Each description SHALL NOT exceed a maximum length of 200 characters.</para>
    /// <para>Description values can contain any UTF-8 characters.</para>
    /// </remarks>
    [JsonPropertyName("alternativeDescriptions")]
    public Dictionary<string, string> IETFLanguageCodesMembers { get; set; }
}
