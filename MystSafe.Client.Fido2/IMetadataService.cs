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
using System.Threading;
using System.Threading.Tasks;

namespace Fido2NetLib;

public interface IMetadataService
{
    /// <summary>
    /// Gets the metadata payload entry by a guid asynchronously
    /// </summary>
    /// <param name="aaguid">The Authenticator Attestation GUID.</param>
    /// <returns>Returns the entry; Otherwise <c>null</c>.</returns>
    Task<MetadataBLOBPayloadEntry?> GetEntryAsync(Guid aaguid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a value indicating whether the internal access token is valid.
    /// </summary>
    /// <returns>
    /// Returns <c>true</c> if access token is valid, or <c>false</c> if the access token is equal to an invalid token value.
    /// </returns>
    bool ConformanceTesting();
}
