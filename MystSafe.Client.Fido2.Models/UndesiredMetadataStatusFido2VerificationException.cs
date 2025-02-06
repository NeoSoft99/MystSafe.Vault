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
using System.Runtime.Serialization;

namespace Fido2NetLib;

/// <summary>
/// Exception thrown when a new attestation comes from an authenticator with a current reported security issue.
/// </summary>
[Serializable]
public class UndesiredMetadataStatusFido2VerificationException : Fido2VerificationException
{
    public UndesiredMetadataStatusFido2VerificationException(StatusReport statusReport) : base($"Authenticator found with undesirable status. Was {statusReport.Status}")
    {
        StatusReport = statusReport;
    }

    protected UndesiredMetadataStatusFido2VerificationException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    /// <summary>
    /// Status report from the authenticator that caused the attestation to be rejected.
    /// </summary>
    public StatusReport StatusReport { get; }
}
