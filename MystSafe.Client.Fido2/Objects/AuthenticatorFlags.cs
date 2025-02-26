﻿// MystSafe is a secret vault with anonymous access and zero activity tracking protected by cryptocurrency-grade tech.
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

namespace Fido2NetLib.Objects;

/// <summary>
/// Authenticator data flags 
/// <see cref="https://www.w3.org/TR/webauthn/#flags"/>
/// </summary>
[Flags]
public enum AuthenticatorFlags : byte
{
    /// <summary>
    /// User Present indicates that the user presence test has completed successfully.
    /// <see cref="https://www.w3.org/TR/webauthn/#up"/>
    /// </summary>
    UP = 0x1,

    /// <summary>
    /// Reserved for future use (RFU1)
    /// </summary>
    RFU1 = 0x2,

    /// <summary>
    /// User Verified indicates that the user verification process has completed successfully.
    /// <see cref="https://www.w3.org/TR/webauthn/#uv"/>
    /// </summary>
    UV = 0x4,

    /// <summary>
    /// A Public Key Credential Source's generating authenticator determines at creation time whether the public key credential source is allowed to be backed up. 
    /// Backup eligibility is signaled in authenticator data's flags along with the current backup state. 
    /// Backup eligibility is a credential property and is permanent for a given public key credential source. 
    /// A backup eligible public key credential source is referred to as a multi-device credential whereas one that is not backup eligible is referred to as a single-device credential.
    /// <see cref="https://w3c.github.io/webauthn/#backup-eligibility"/>
    /// </summary>
    BE = 0x8,

    /// <summary>
    /// The current backup state of a multi-device credential as determined by the current managing authenticator. 
    /// Backup state is signaled in authenticator data's flags and can change over time.
    /// <see cref="https://w3c.github.io/webauthn/#backup-state"/>
    /// </summary>
    BS = 0x10,

    /// <summary>
    /// Reserved for future use (RFU4)
    /// </summary>
    RFU4 = 0x20,

    /// <summary>
    /// Attested credential data included indicates that the authenticator added attested credential data to the authenticator data.
    /// <see cref="https://www.w3.org/TR/webauthn/#attested-credential-data"/>
    /// </summary>
    AT = 0x40,

    /// <summary>
    /// Extension data included indicates that the authenticator added extension data to the authenticator data.
    /// <see cref="https://www.w3.org/TR/webauthn/#authdataextensions"/>
    /// </summary>
    ED = 0x80,
}
