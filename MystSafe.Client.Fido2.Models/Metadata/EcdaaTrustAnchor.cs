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
/// Represents the the ECDAA attestation data.
/// </summary>
/// <remarks>
/// <see href="https://fidoalliance.org/specs/fido-v2.0-rd-20180702/fido-metadata-statement-v2.0-rd-20180702.html#ecdaatrustanchor-dictionary"/>
/// <para>In the case of ECDAA attestation, the ECDAA-Issuer's trust anchor MUST be specified in this field.</para>
/// </remarks>
public sealed class EcdaaTrustAnchor
{
    /// <summary>
    /// Gets or sets a base64url encoding of the result of ECPoint2ToB of the ECPoint2 X=P2​x​​.
    /// </summary>
    [JsonPropertyName("x"), Required]
    public string X { get; set; }

    /// <summary>
    /// Gets or sets a base64url encoding of the result of ECPoint2ToB of the ECPoint2.
    /// </summary>
    [JsonPropertyName("y"), Required]
    public string Y { get; set; }

    /// <summary>
    /// Gets or sets a base64url encoding of the result of BigNumberToB(c).
    /// </summary>
    [JsonPropertyName("c"), Required]
    public string C { get; set; }

    /// <summary>
    /// Gets or sets the base64url encoding of the result of BigNumberToB(sx).
    /// </summary>
    [JsonPropertyName("sx"), Required]
    public string SX { get; set; }

    /// <summary>
    /// Gets or sets the base64url encoding of the result of BigNumberToB(sy).
    /// </summary>
    [JsonPropertyName("sy"), Required]
    public string SY { get; set; }

    /// <summary>
    /// Gets or sets a name of the Barreto-Naehrig elliptic curve for G1.
    /// <para>"BN_P256", "BN_P638", "BN_ISOP256", and "BN_ISOP512" are supported.</para>
    /// </summary>
    [JsonPropertyName("G1Curve"), Required]
    public string G1Curve { get; set; }
}
