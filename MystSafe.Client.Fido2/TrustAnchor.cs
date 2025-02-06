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

using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

using Fido2NetLib.Exceptions;

namespace Fido2NetLib;

public static class TrustAnchor
{
    public static void Verify(MetadataBLOBPayloadEntry? metadataEntry, X509Certificate2[] trustPath)
    {
        if (trustPath != null && metadataEntry?.MetadataStatement?.AttestationTypes is not null)
        {
            static bool ContainsAttestationType(MetadataBLOBPayloadEntry entry, MetadataAttestationType type)
            {
                return entry.MetadataStatement.AttestationTypes.Contains(type.ToEnumMemberValue());
            }

            // If the authenticator's metadata requires basic full attestation, build and verify the chain
            if (ContainsAttestationType(metadataEntry, MetadataAttestationType.ATTESTATION_BASIC_FULL) ||
                ContainsAttestationType(metadataEntry, MetadataAttestationType.ATTESTATION_PRIVACY_CA))
            {
                string[] certStrings = metadataEntry.MetadataStatement.AttestationRootCertificates;
                var attestationRootCertificates = new X509Certificate2[certStrings.Length];

                for (int i = 0; i < attestationRootCertificates.Length; i++)
                {
                    attestationRootCertificates[i] = new X509Certificate2(Convert.FromBase64String(certStrings[i]));
                }

                if (!CryptoUtils.ValidateTrustChain(trustPath, attestationRootCertificates))
                {
                    throw new Fido2VerificationException(Fido2ErrorMessages.InvalidCertificateChain);
                }
            }

            else if (ContainsAttestationType(metadataEntry, MetadataAttestationType.ATTESTATION_ANONCA))
            {
                // skip verification for Anonymization CA (AnonCA)
            }
            else // otherwise, ensure the certificate is self signed
            {
                var trustPath0 = trustPath[0];

                if (!string.Equals(trustPath0.Subject, trustPath0.Issuer, StringComparison.Ordinal))
                {
                    // TODO: Improve this error message
                    throw new Fido2VerificationException("Attestation with full attestation from authenticator that does not support full attestation");
                }
            }

            // TODO: Verify all MetadataAttestationTypes are correctly handled

            // [ ] ATTESTATION_ECDAA "ecdaa"    | currently handled as self signed  w/ no test coverage
            // [ ] ATTESTATION_ANONCA "anonca"  | currently not verified            w/ no test coverage
            // [ ] ATTESTATION_NONE "none"      | currently handled as self signed  w/ no test coverage               
        }
    }
}
