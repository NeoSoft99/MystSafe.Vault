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
using System.Formats.Cbor;

namespace Fido2NetLib.Cbor;

public abstract class CborObject
{
    public abstract CborType Type { get; }

    public static CborObject Decode(ReadOnlyMemory<byte> data)
    {
        var reader = new CborReader(data);

        return Read(reader);
    }

    public static CborObject Decode(ReadOnlyMemory<byte> data, out int bytesRead)
    {
        var reader = new CborReader(data);

        var result = Read(reader);

        bytesRead = data.Length - reader.BytesRemaining;

        return result;
    }

    public virtual CborObject this[int index] => null!;

    public virtual CborObject? this[string name] => null;

    public static explicit operator string(CborObject obj)
    {
        return ((CborTextString)obj).Value;
    }

    public static explicit operator byte[](CborObject obj)
    {
        return ((CborByteString)obj).Value;
    }

    public static explicit operator int(CborObject obj)
    {
        return (int)((CborInteger)obj).Value;
    }

    public static explicit operator long(CborObject obj)
    {
        return ((CborInteger)obj).Value;
    }

    public static explicit operator bool(CborObject obj)
    {
        return ((CborBoolean)obj).Value;
    }

    private static CborObject Read(CborReader reader)
    {
        CborReaderState s = reader.PeekState();

        return s switch
        {
            CborReaderState.StartMap => ReadMap(reader),
            CborReaderState.StartArray => ReadArray(reader),
            CborReaderState.TextString => new CborTextString(reader.ReadTextString()),
            CborReaderState.Boolean => (CborBoolean)reader.ReadBoolean(),
            CborReaderState.ByteString => new CborByteString(reader.ReadByteString()),
            CborReaderState.UnsignedInteger => new CborInteger(reader.ReadInt64()),
            CborReaderState.NegativeInteger => new CborInteger(reader.ReadInt64()),
            CborReaderState.Null => ReadNull(reader),
            _ => throw new Exception($"Unhandled state. Was {s}")
        };
    }

    private static CborNull ReadNull(CborReader reader)
    {
        reader.ReadNull();

        return CborNull.Instance;
    }

    private static CborArray ReadArray(CborReader reader)
    {
        int? count = reader.ReadStartArray();

        var items = count != null
            ? new List<CborObject>(count.Value)
            : new List<CborObject>();

        while (!(reader.PeekState() is CborReaderState.EndArray or CborReaderState.Finished))
        {
            items.Add(Read(reader));
        }

        reader.ReadEndArray();

        return new CborArray(items);
    }

    private static CborMap ReadMap(CborReader reader)
    {
        int? count = reader.ReadStartMap();

        var map = count.HasValue ? new CborMap(count.Value) : new CborMap();

        while (!(reader.PeekState() is CborReaderState.EndMap or CborReaderState.Finished))
        {
            CborObject k = Read(reader);
            CborObject v = Read(reader);

            map.Add(k, v);
        }

        reader.ReadEndMap();

        return map;
    }

    public byte[] Encode()
    {
        var writer = new CborWriter();

        writer.WriteObject(this);

        return writer.Encode();
    }
}
