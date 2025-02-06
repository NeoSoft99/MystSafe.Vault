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

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Fido2NetLib;

[JsonConverter(typeof(FidoEnumConverter<MetadataAttestationType>))]
internal enum MetadataAttestationType
{
    /// <summary>
    /// Indicates full basic attestation, based on an attestation private key shared among a class of authenticators (e.g. same model). 
    /// Authenticators must provide its attestation signature during the registration process for the same reason. 
    /// The attestation trust anchor is shared with FIDO Servers out of band (as part of the Metadata). 
    /// This sharing process shouldt be done according to [UAFMetadataService].
    /// </summary>
    [EnumMember(Value = "basic_full")]
    ATTESTATION_BASIC_FULL = 0x3e07,

    /// <summary>
    /// Just syntactically a Basic Attestation.
    /// The attestation object self-signed, i.e. it is signed using the UAuth.priv key, i.e. the key corresponding to the UAuth.pub key included in the attestation object. 
    /// As a consequence it does not provide a cryptographic proof of the security characteristics. 
    /// But it is the best thing we can do if the authenticator is not able to have an attestation private key.
    /// </summary>
    [EnumMember(Value = "basic_surrogate")]
    ATTESTATION_BASIC_SURROGATE = 0x3e08,

    /// <summary>
    /// Indicates use of elliptic curve based direct anonymous attestation as defined in [FIDOEcdaaAlgorithm].
    /// </summary>
    [EnumMember(Value = "ecdaa")]
    [Fido2Standard(Optional = true)]
    ATTESTATION_ECDAA = 0x3e09,

    /// <summary>
    /// Indicates PrivacyCA attestation as defined in [TCG-CMCProfile-AIKCertEnroll]. 
    /// </summary>
    [EnumMember(Value = "attca")]
    [Fido2Standard(Optional = true)]
    ATTESTATION_PRIVACY_CA = 0x3e10,

    /// <summary>
    /// Anonymization CA (AnonCA)
    /// </summary>
    [EnumMember(Value = "anonca")]
    ATTESTATION_ANONCA = 0x3e0c,

    [EnumMember(Value = "none")]
    ATTESTATION_NONE = 0x3e0b
}
