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
using System.Buffers;
using System.Buffers.Binary;

namespace Fido2NetLib;

internal static class IBufferWriterExtensions
{
    public static void WriteUInt16BigEndian(this IBufferWriter<byte> writer, ushort value)
    {
        var buffer = writer.GetSpan(2);

        BinaryPrimitives.WriteUInt16BigEndian(buffer, value);

        writer.Advance(2);
    }

    public static void WriteUInt32BigEndian(this IBufferWriter<byte> writer, uint value)
    {
        var buffer = writer.GetSpan(4);

        BinaryPrimitives.WriteUInt32BigEndian(buffer, value);

        writer.Advance(4);
    }

    public static void WriteGuidBigEndian(this IBufferWriter<byte> writer, Guid value)
    {
        var buffer = writer.GetSpan(16);

        _ = value.TryWriteBytes(buffer);

        if (BitConverter.IsLittleEndian)
        {
            SwapBytes(buffer, 0, 3);
            SwapBytes(buffer, 1, 2);
            SwapBytes(buffer, 4, 5);
            SwapBytes(buffer, 6, 7);
        }

        writer.Advance(16);
    }

    private static void SwapBytes(Span<byte> bytes, int index1, int index2)
    {
        var temp = bytes[index1];
        bytes[index1] = bytes[index2];
        bytes[index2] = temp;
    }
}
