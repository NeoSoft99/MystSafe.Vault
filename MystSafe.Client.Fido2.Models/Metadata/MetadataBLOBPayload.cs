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
/// Represents the MetadataBLOBPayload
/// </summary>
/// <remarks>
/// <see xref="https://fidoalliance.org/specs/mds/fido-metadata-service-v3.0-ps-20210518.html#metadata-blob-payload-dictionary"/>
/// </remarks>
public sealed class MetadataBLOBPayload
{
    /// <summary>
    /// Gets or sets the legalHeader, if present, contains a legal guide for accessing and using metadata.
    /// </summary>
    /// <remarks>
    /// This value MAY contain URL(s) pointing to further information, such as a full Terms and Conditions statement. 
    /// </remarks>
    [JsonPropertyName("legalHeader")]
    public string LegalHeader { get; set; }

    /// <summary>   
    /// Gets or sets the serial number of this UAF Metadata BLOB Payload. 
    /// </summary>
    /// <remarks>
    /// Serial numbers MUST be consecutive and strictly monotonic, i.e. the successor BLOB will have a no value exactly incremented by one.
    /// </remarks>
    [JsonPropertyName("no"), Required]
    public int Number { get; set; }

    /// <summary>
    /// Gets or sets a formatted date (ISO-8601) when the next update will be provided at latest.
    /// </summary>
    [JsonPropertyName("nextUpdate"), Required]
    public string NextUpdate { get; set; }

    /// <summary>
    /// Gets or sets a list of zero or more entries of <see cref="MetadataBLOBPayloadEntry"/>.
    /// </summary>
    [JsonPropertyName("entries"), Required]
    public MetadataBLOBPayloadEntry[] Entries { get; set; }

    /// <summary>
    /// The "alg" property from the original JWT header. Used to validate MetadataStatements.
    /// </summary>
    [JsonPropertyName("jwtAlg")]
    public string JwtAlg { get; set; }
}
