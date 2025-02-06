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

namespace Fido2NetLib.Exceptions;

[Flags]
public enum Fido2ErrorCode
{
    Unknown = 0,
    InvalidRpidHash,
    InvalidSignature,
    InvalidSignCount,
    UserVerificationRequirementNotMet,
    UserPresentFlagNotSet,
    UnexpectedExtensions,
    MissingStoredPublicKey,
    InvalidAttestation,
    InvalidAttestationObject,
    MalformedAttestationObject,
    AttestedCredentialDataFlagNotSet,
    UnknownAttestationType,
    MissingAttestationType,
    MalformedExtensionsDetected,
    UnexpectedExtensionsDetected,
    InvalidAssertionResponse,
    InvalidAttestationResponse,
    InvalidAttestedCredentialData,
    InvalidAuthenticatorResponse,
    MalformedAuthenticatorResponse,
    MissingAuthenticatorData,
    InvalidAuthenticatorData,
    MissingAuthenticatorResponseChallenge,
    InvalidAuthenticatorResponseChallenge,
    NonUniqueCredentialId,
    AaGuidNotFound,
    UnimplementedAlgorithm,
    BackupEligibilityRequirementNotMet,
    BackupStateRequirementNotMet,
    CredentialAlgorithmRequirementNotMet,
    DevicePublicKeyAuthentication
}
