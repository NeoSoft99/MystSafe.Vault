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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Fido2NetLib.Serialization;

namespace Fido2NetLib;

public sealed class FileSystemMetadataRepository : IMetadataRepository
{
    private readonly string _path;
    private readonly IDictionary<Guid, MetadataBLOBPayloadEntry> _entries;
    private MetadataBLOBPayload? _blob;

    public FileSystemMetadataRepository(string path)
    {
        _path = path;
        _entries = new Dictionary<Guid, MetadataBLOBPayloadEntry>();
    }

    public async Task<MetadataStatement?> GetMetadataStatementAsync(MetadataBLOBPayload blob, MetadataBLOBPayloadEntry entry, CancellationToken cancellationToken = default)
    {
        if (_blob is null)
            await GetBLOBAsync(cancellationToken);

        if (entry.AaGuid is Guid aaGuid && _entries.TryGetValue(aaGuid, out var found))
        {
            return found.MetadataStatement;
        }

        return null;
    }

    public async Task<MetadataBLOBPayload> GetBLOBAsync(CancellationToken cancellationToken = default)
    {
        if (Directory.Exists(_path))
        {
            foreach (var filename in Directory.GetFiles(_path))
            {
                await using var fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                MetadataStatement statement = await JsonSerializer.DeserializeAsync(fileStream, FidoModelSerializerContext.Default.MetadataStatement, cancellationToken: cancellationToken) ?? throw new NullReferenceException(nameof(statement));
                var conformanceEntry = new MetadataBLOBPayloadEntry
                {
                    AaGuid = statement.AaGuid,
                    MetadataStatement = statement,
                    StatusReports = new StatusReport[]
                    {
                        new StatusReport
                        {
                            Status = AuthenticatorStatus.NOT_FIDO_CERTIFIED
                        }
                    }
                };
                if (null != conformanceEntry.AaGuid)
                    _entries.Add(conformanceEntry.AaGuid.Value, conformanceEntry);
            }
        }

        _blob = new MetadataBLOBPayload()
        {
            Entries = _entries.Select(static o => o.Value).ToArray(),
            NextUpdate = "", //Empty means it won't get cached
            LegalHeader = "Local FAKE",
            Number = 1
        };

        return _blob;
    }
}
