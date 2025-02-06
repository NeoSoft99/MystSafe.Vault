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
using System.Formats.Cbor;

namespace Fido2NetLib.Cbor;

internal static class CborWriterExtensions
{
    public static void WriteObject(this CborWriter writer, CborObject @object)
    {
        if (@object is CborTextString text)
        {
            writer.WriteTextString(text.Value);
        }
        else if (@object is CborByteString data)
        {
            writer.WriteByteString(data.Value);
        }
        else if (@object is CborBoolean boolean)
        {
            writer.WriteBoolean(boolean.Value);
        }
        else if (@object is CborInteger number)
        {
            writer.WriteInt64(number.Value);
        }
        else if (@object is CborMap map)
        {
            writer.WriteMap(map);
        }
        else if (@object is CborArray array)
        {
            writer.WriteArray(array);
        }
        else if (@object.Type == CborType.Null)
        {
            writer.WriteNull();
        }
        else
        {
            throw new Exception($"Unknown type. Was  {@object.Type}");
        }
    }

    public static void WriteArray(this CborWriter writer, CborArray array)
    {
        writer.WriteStartArray(array.Length);

        foreach (var item in array.Values)
        {
            WriteObject(writer, item);
        }

        writer.WriteEndArray();
    }

    public static void WriteMap(this CborWriter writer, CborMap map)
    {
        writer.WriteStartMap(map.Count);

        foreach (var item in map)
        {
            WriteObject(writer, item.Key);
            WriteObject(writer, item.Value);
        }

        writer.WriteEndMap();
    }
}
